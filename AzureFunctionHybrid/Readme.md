# Project Azure function Hybrid cconnection manager
In that solution, I've build an IasC solution to implement an Azure Function with an Azure Hybrid connection enabled.
At the moment there isn't any powershell module available, that's the reason, why I've created an Azure ARM template.

## Getting Started
Bevor you start, check out my Blog Post to get more infomation about the solution and also the need of the hybrid connection manager.
Blog Post: https://www.cloudblogger.at/2019/12/14/function-get-data-from-on-prem/

### Prerequisites
To implement that solution the following prerequisites are required:
* Active Azure Subscription
* One Server On-Prem with the following firewall prerequisits:
    * Port 80,443 outgoing to the Hybrid Manager relay (*.servicebus.windows.net)
    * Azure Hybrid Connection manager can only be installed on Windows OS!

### Installation
At the Azure Portal open the section "Template deployment (deploy using custom templates)" and select "Build your own template in the editor".
Otherwise, there are different options via Powerhell, Azure DevOps,... available

### Authors
Hannes Lagler-Gruener

### Future Versions
* Azure powershell script implementation, when the options are in place by Microsoft

<a href="https://azuredeploy.net/?repository=https://github.com/laglergruener/AzureARMTemplates/tree/master/AzureFunctionHybrid" target="_blank">
    <img src="https://azurecomcdn.azureedge.net/mediahandler/acomblog/media/Default/blog/deploybutton.png"/>
</a>
