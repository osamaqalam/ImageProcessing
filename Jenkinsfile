pipeline {
    agent any
    
    environment {
        DOTNET_CLI_HOME = "${WORKSPACE}/.dotnet"
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out code...'
                checkout scm
            }
        }
        
        stage('Restore Dependencies') {
            steps {
                echo 'Restoring NuGet packages...'
                bat 'dotnet restore ImageProcessing.sln'
            }
        }
        
        stage('Build') {
            steps {
                echo 'Building the solution...'
                bat 'dotnet build ImageProcessing.sln --configuration Release --no-restore'
            }
        }
        
        stage('Run Tests') {
            steps {
                echo 'Running unit tests...'
                bat 'dotnet test ImageProcessing.sln --configuration Release --no-build --verbosity normal --logger "trx;LogFileName=test_results.trx"'
            }
        }
    }
    
    post {
        always {
            echo 'Publishing test results...'
            // Archive test results
            archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
            
            // Publish test results to Jenkins
            mstest testResultsFile: '**/TestResults/*.trx'
        }
        success {
            echo 'Build and tests succeeded! ✓'
        }
        failure {
            echo 'Build or tests failed! ✗'
        }
    }
}
