using ImageProcessing.App.Models.Flowchart;
using ImageProcessing.App.ViewModels;
using ImageProcessing.App.ViewModels.Flowchart;
using ImageProcessing.App.ViewModels.Flowchart.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ImageProcessing.App.Services
{
    /// <summary>
    /// Service for serializing and deserializing flowcharts
    /// </summary>
    public interface IFlowchartSerializationService
    {
        void SaveFlowchart(string filePath, ObservableCollection<IFlowchartNode> nodes, ObservableCollection<ConnectionViewModel> connections);
        FlowchartDTO LoadFlowchart(string filePath);
    }

    public class FlowchartSerializationService : IFlowchartSerializationService
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public FlowchartSerializationService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                // Formats the JSON with indentation and line breaks for readability
                WriteIndented = true,
                // Allows case-insensitive property matching during deserialization
                PropertyNameCaseInsensitive = true,
                // Handles enum values as strings in JSON
                Converters = { new JsonStringEnumConverter() }
            };
        }

        public void SaveFlowchart(string filePath, ObservableCollection<IFlowchartNode> nodes, ObservableCollection<ConnectionViewModel> connections)
        {
            var flowchartDto = new FlowchartDTO();

            // Convert nodes to DTOs
            foreach (var node in nodes)
            {
                if (node is FlowchartNodeViewModel vm)
                {
                    var nodeDto = new NodeDTO
                    {
                        Id = vm.Id,
                        NodeType = GetNodeType(vm),
                        Label = vm.Label,
                        X = vm.X,
                        Y = vm.Y,
                        Width = vm.Width,
                        Height = vm.Height,
                        Properties = ExtractNodeProperties(vm)
                    };
                    flowchartDto.Nodes.Add(nodeDto);
                }
            }

            // Convert connections to DTOs
            foreach (var connection in connections)
            {
                if (connection.Source is FlowchartNodeViewModel source && 
                    connection.Target is FlowchartNodeViewModel target)
                {
                    flowchartDto.Connections.Add(new ConnectionDTO
                    {
                        SourceNodeId = source.Id,
                        TargetNodeId = target.Id
                    });
                }
            }

            // Serialize and save
            var json = JsonSerializer.Serialize(flowchartDto, _jsonOptions);
            File.WriteAllText(filePath, json);
        }

        public FlowchartDTO LoadFlowchart(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var flowchartDto = JsonSerializer.Deserialize<FlowchartDTO>(json, _jsonOptions);
            
            if (flowchartDto == null)
                throw new InvalidOperationException("Failed to deserialize flowchart file.");

            return flowchartDto;
        }

        private string GetNodeType(FlowchartNodeViewModel vm)
        {
            return vm switch
            {
                StartNodeViewModel => "Start",
                EndNodeViewModel => "End",
                LoadImageNodeViewModel => "LoadImage",
                GrayscaleNodeViewModel => "Grayscale",
                ResizeNodeViewModel => "Resize",
                BinarizeNodeViewModel => "Binarize",
                _ => "Unknown"
            };
        }

        private Dictionary<string, object> ExtractNodeProperties(FlowchartNodeViewModel vm)
        {
            var properties = new Dictionary<string, object>();

            switch (vm)
            {
                case LoadImageNodeViewModel loadNode:
                    if (!string.IsNullOrEmpty(loadNode.ImagePath))
                        properties["ImagePath"] = loadNode.ImagePath;
                    break;

                case GrayscaleNodeViewModel grayscaleNode:
                    if (!string.IsNullOrEmpty(grayscaleNode.SelectedInImgLabel))
                        properties["SelectedInImgLabel"] = grayscaleNode.SelectedInImgLabel;
                    break;

                case ResizeNodeViewModel resizeNode:
                    if (!string.IsNullOrEmpty(resizeNode.SelectedInImgLabel))
                        properties["SelectedInImgLabel"] = resizeNode.SelectedInImgLabel;
                    if (!string.IsNullOrEmpty(resizeNode.SelectedInterpolationMode))
                        properties["SelectedInterpolationMode"] = resizeNode.SelectedInterpolationMode;
                    properties["Scale"] = resizeNode.Scale;
                    break;

                case BinarizeNodeViewModel binarizeNode:
                    if (!string.IsNullOrEmpty(binarizeNode.SelectedInImgLabel))
                        properties["SelectedInImgLabel"] = binarizeNode.SelectedInImgLabel;
                    if (!string.IsNullOrEmpty(binarizeNode.SelectedThresholdingType))
                        properties["SelectedThresholdingType"] = binarizeNode.SelectedThresholdingType;
                    properties["RangeStart"] = binarizeNode.RangeStart;
                    properties["RangeEnd"] = binarizeNode.RangeEnd;
                    break;
            }

            return properties;
        }
    }
}
