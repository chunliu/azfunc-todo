# azfunc-todo
![CI Build](https://github.com/chunliu/azfunc-todo/workflows/CI%20Build/badge.svg)  ![Deploy](https://github.com/chunliu/azfunc-todo/workflows/Deploy/badge.svg)

The code in this repo implements REST APIs for CRUD of a todo list with Azure Functions. It is a sample to demonstrate the following features. 

- Developing Azure Function apps with Visual Studio Code/Codespaces.
- Using Entity Framework Core to connect to Azure SQL in Function apps.  
- Building and deploying Function apps with GitHub workflow.

## Deployment with ARM template

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fchunliu%2Fazfunc-todo%2Fmaster%2Farmtemplate%2Fazuredeploy.json)

The ARM template will create an environment with the following Azure resources and deploy the function app to it.

- An Azure SQL database.
- A Function app hosted in an App Service Plan.
- A storage account and an appinsight tenant to support the function app.
- An API Management instance with the API operations defined.
- A vnet to connect all the above resources together.
- The connection is restricted to APIM -> Function App -> Azure Sql. The APIs can only be called via APIM. 

![arm diagram](/media/arm-diagram.png)