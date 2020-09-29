<#
    SYNOPSIS
        This script is used to demonstrate an azure PSE Architecture
    .DESCRIPTION
        This script is used to demonstrate an azure PSE Architecture
    .EXAMPLE
        
    .NOTES  
        Please install the latest Az Modules!
        Please also install AzureRM.Profile module
        !!Important!!, the HybridConnection implementation isn't possible at the moment! 
        User Voice entry: https://feedback.azure.com/forums/169385-web-apps/suggestions/33850393-programmatically-associating-a-hybrid-connection-t        
#>

#######################################################################################################################
#region define global variables

    $resourcegroup = "ACP-ServerLess-Demo"
    $location = "West Europe"

    $functionname = "ACP-Demo-Function"
    $appsvcplanname = "appsvcplandemofunctionapp"
    $storageaccname = "straccdemofunctionapp"
    $appinsightname = "appinsightdemofunction"

    $servicebusrelayname = "demohconnection"
    $hybridconnectioname = "democonnection"

    $ErrorActionPreference = 'Stop'

#endregion
#######################################################################################################################


#######################################################################################################################
#region Functions

    #login to azure and get access token
    function Login-Azure()
    {
        try 
        {
            if(-not (Get-Module Az.Accounts)) {
                Import-Module Az.Accounts
            }
        
            Connect-AzAccount               
        }
        catch {
            Write-Error "Error in function Login-Azure. Error message: $($_.Exception.Message)"
        }
    }

#endregion
#######################################################################################################################


#######################################################################################################################
#region Script start

    Write-Host "Connect to Azure"
    Login-Azure

    #region section select subscription
    try 
    {            
        $subscriptions = Get-AzSubscription

        if (($subscriptions).count -gt 0)
        {
            Write-Host "#######################################################################"
            Write-Host "There are more subscription available:"

            $count = 0
            foreach ($subscription in $subscriptions) 
            {
                Write-Host "$($count): $($subscription.Name)"
                $count++
            }

            Write-Host "Please select the right subscription (insert the number)"
            Write-Host "#######################################################################"
            $result = Read-Host

            $selectedsubscription = $subscriptions[$result]
            Select-AzSubscription -SubscriptionObject $selectedsubscription
        }
        else 
        {
            $selectedsubscription = $subscriptions[0]
            Select-AzSubscription -SubscriptionObject $selectedsubscription
        }
    }
    catch {
        Write-Error "Error in select ressourcegroup section. Error message: $($_.Exception.Message)"
    }

    #endregion

    #Create ResourceGroup if not exist
    #---------------------------------------------------------------
    try 
    {            
        Write-Host "Create ResourceGroup $($resourcegroup) if not exist."

        $rg = Get-AzResourceGroup | Where-Object ResourceGroupName -EQ $resourcegroup
        if($null -eq $rg)
        {
            $rg = New-AzResourceGroup -Name $resourcegroup -Location $location
        }
    }
    catch {
        Write-Error "Error in create ressourcegroup section. Error message: $($_.Exception.Message)"
    }

    #Create Azure function
    #---------------------------------------------------------------    
    $appsvcplan = New-AzAppServicePlan -Name $appsvcplanname -Tier Premium -Location $location -ResourceGroupName $resourcegroup

    $storageacc = New-AzStorageAccount -Name $storageaccname -SkuName Standard_LRS -ResourceGroupName $resourcegroup -Location $location

    $appinsight = New-AzApplicationInsights -Name $appinsightname -Location $location -ResourceGroupName $resourcegroup -Kind web

    $storageaccountkey = $storageacc | Get-AzStorageAccountKey | Where-Object { $_.KeyName -eq "Key1" } | Select-Object Value
    $storageaccountconnectionstring = "DefaultEndpointsProtocol=https;AccountName=$($storageacc.Name);AccountKey=$($storageaccountkey.Value)"
    
    $FunctionAppSettings = @{
        ServerFarmId="/subscriptions/<GUID>/resourceGroups/$resourcegroup/providers/Microsoft.Web/serverfarms/$($appsvcplan.Name)";
        alwaysOn=$True;
    }
    
    $azurefunction = New-AzResource -ResourceGroupName $resourcegroup -Location $location -ResourceName $functionname `
                                    -ResourceType "microsoft.web/sites" -Kind "functionapp" `
                                    -Properties $FunctionAppSettings -Force

    $AzFunctionAppSettings = @{
        APPINSIGHTS_INSTRUMENTATIONKEY = $appinsight.InstrumentationKey;
        AzureWebJobsDashboard = $storageaccountconnectionstring;
        AzureWebJobsStorage = $storageaccountconnectionstring;
        FUNCTIONS_EXTENSION_VERSION = "~2";
        FUNCTIONS_WORKER_RUNTIME = "dotnet";
    }

    # Set the correct application settings on the function app
    Set-AzWebApp -Name $functionname -ResourceGroupName $resourcegroup -AppSettings $AzFunctionAppSettings    
    
    $azrelaynamespace = New-AzRelayNamespace -ResourceGroupName $resourcegroup -Name $servicebusrelayname -Location $location
     
    properties = @{
        "requiresClientAuthorization": true,
        "userMetadata": "[[{\"key\":\"endpoint\",\"value\":\"localhost:80\"}]"
    }
    
    $temp2 = Get-AzWebApp -ResourceGroupName $resourcegroup -Name $functionname




    $getHybirdConnection = Get-AzRelayHybridConnection -ResourceGroupName $resourcegroup -NamespaceName $servicebusrelayname -Name $hybridconnectioname
    $getHybirdConnection.UserMetadata = "TestHybirdConnection2"
    $getHybirdConnection.RequiresClientAuthorization = $False
    $azhybridconnection = New-AzRelayHybridConnection -ResourceGroupName $resourcegroup -Namespace $azrelaynamespace.Name -Name $hybridconnectioname -InputObject $null

#endregion
#######################################################################################################################