using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.Utilities;
using ImageProcessing.App.ViewModels;
using ImageProcessing.App.ViewModels.Flowchart;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace ImageProcessing.App.Services
{
    /// <summary>
    /// Factory for creating node ViewModels from DTOs
    /// </summary>
    public class NodeFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ObservableDictionary<string, ImageNodeData> _outputImages;

        public NodeFactory(IServiceProvider serviceProvider, ObservableDictionary<string, ImageNodeData> outputImages)
        {
            _serviceProvider = serviceProvider;
            _outputImages = outputImages;
        }

        public IFlowchartNode? CreateNode(NodeDTO dto)
        {
            IFlowchartNode? node = dto.NodeType switch
            {
                "Start" => new StartNodeViewModel(),
                "End" => new EndNodeViewModel(),
                "LoadImage" => _serviceProvider.GetRequiredService<LoadImageNodeViewModel>(),
                "Grayscale" => new GrayscaleNodeViewModel(
                    _serviceProvider.GetRequiredService<Services.Imaging.IImageService>(),
                    _outputImages),
                "Resize" => new ResizeNodeViewModel(
                    _serviceProvider.GetRequiredService<Services.Imaging.IImageService>(),
                    _outputImages),
                "Binarize" => new BinarizeNodeViewModel(
                    _serviceProvider.GetRequiredService<Services.Imaging.IImageService>(),
                    _outputImages),
                _ => null
            };

            if (node is FlowchartNodeViewModel vm)
            {
                // Restore common properties
                vm.Id = dto.Id;
                vm.Label = dto.Label;
                vm.X = dto.X;
                vm.Y = dto.Y;
                vm.Width = dto.Width;
                vm.Height = dto.Height;

                // Restore node-specific properties
                RestoreNodeProperties(vm, dto);
            }

            return node;
        }

        private void RestoreNodeProperties(FlowchartNodeViewModel vm, NodeDTO dto)
        {
            switch (vm)
            {
                case LoadImageNodeViewModel loadNode:
                    if (dto.Properties.TryGetValue("ImagePath", out var imagePath))
                        loadNode.ImagePath = GetStringValue(imagePath);
                    break;

                case GrayscaleNodeViewModel grayscaleNode:
                    if (dto.Properties.TryGetValue("SelectedInImgLabel", out var grayInImg))
                        grayscaleNode.SelectedInImgLabel = GetStringValue(grayInImg);
                    break;

                case ResizeNodeViewModel resizeNode:
                    if (dto.Properties.TryGetValue("SelectedInImgLabel", out var resizeInImg))
                        resizeNode.SelectedInImgLabel = GetStringValue(resizeInImg);
                    if (dto.Properties.TryGetValue("SelectedInterpolationMode", out var interpMode))
                        resizeNode.SelectedInterpolationMode = GetStringValue(interpMode);
                    if (dto.Properties.TryGetValue("Scale", out var scale))
                        resizeNode.Scale = GetDoubleValue(scale);
                    break;

                case BinarizeNodeViewModel binarizeNode:
                    if (dto.Properties.TryGetValue("SelectedInImgLabel", out var binInImg))
                        binarizeNode.SelectedInImgLabel = GetStringValue(binInImg);
                    if (dto.Properties.TryGetValue("SelectedThresholdingType", out var threshType))
                        binarizeNode.SelectedThresholdingType = GetStringValue(threshType);
                    if (dto.Properties.TryGetValue("RangeStart", out var rangeStart))
                        binarizeNode.RangeStart = GetInt32Value(rangeStart);
                    if (dto.Properties.TryGetValue("RangeEnd", out var rangeEnd))
                        binarizeNode.RangeEnd = GetInt32Value(rangeEnd);
                    break;
            }
        }

        private string? GetStringValue(object? value)
        {
            if (value is JsonElement jsonElement)
                return jsonElement.ValueKind == JsonValueKind.String ? jsonElement.GetString() : null;
            return value?.ToString();
        }

        private double GetDoubleValue(object? value)
        {
            if (value is JsonElement jsonElement)
                return jsonElement.GetDouble();
            return Convert.ToDouble(value);
        }

        private int GetInt32Value(object? value)
        {
            if (value is JsonElement jsonElement)
                return jsonElement.GetInt32();
            return Convert.ToInt32(value);
        }
    }
}
