using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using System.Collections.Generic;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

using Pulumi.AzureNative.CostManagement;

return await Pulumi.Deployment.RunAsync(() =>
{

    // Create an instance of ResourceGroup component
    var resourceGroup = new myResourceGroup("myResourceGroup", "test", "UkSouth");

    // Create an Azure resource (Storage Account)
    var storageAccount = new myStorageAccount("sa002qw");

    // Create Virtual Network
    var virtualNetwork = new myVirtualNetwork("Vnet1", "UkSouth", "test", "10.10.100.0/24");

    // Create subnets
    var subnet1 = new mySubnet("subnet1", "10.10.100.32/27", "test", "subnet1", "vnet1");
    var subnet2 = new mySubnet("subnet2", "10.10.100.64/27", "test", "subnet2", "vnet1");

    // Create NSG
    var networkSecurityGroupResource = new myNsg("testNsg", "test", "UkSouth");

    //create App service Plan
    var functionAppServicePlan = new myAppServicePlan("UKSouth", "functionAppServicePlan", "test", "functionAppServicePlan");
    var webAppServicePlan      = new myAppServicePlan("UKSouth", "webAppServicePlan", "test", "webAppServicePlan");

    //create web app
   // var webApp = new myWebApp("UKSouth", "test", "webApp", "webApp");
});


public class myResourceGroup
{
    private readonly string _name;
    private readonly string _resourceGroupName;
    private readonly string _location;


    public myResourceGroup(string name, string resourceGroupName, string location)
    {
        _name = name;
        _resourceGroupName = resourceGroupName;
        _location = location;

        var resourceGroup = new ResourceGroup(name, new ResourceGroupArgs
        {
            ResourceGroupName = resourceGroupName,
            Location = location
        });
    }

}

public class myStorageAccount
{
    private readonly string _name;
   // private readonly string _resourceGroup;
   // private readonly string _location;

    public myStorageAccount(string name)
    {
        _name = name;

        var resourceGroup = new ResourceGroup("test", new()
        {  
            ResourceGroupName = "test",
            Location = "UKSouth",
        });

        var storageAccountArgs = new StorageAccountArgs
        {
            AccountName = name,
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Kind = Kind.StorageV2,
            Sku = new SkuArgs { Name = SkuName.Standard_LRS }
        };

        var storageAccount = new StorageAccount(name, storageAccountArgs);
    }
}

public class myVirtualNetwork
{
    private readonly string _virtualNetworkName;
    private readonly string _location;
    private readonly string _resourceGroup;
    private readonly string _AddressPrefixes;

    public myVirtualNetwork(string virtualNetworkName, string location, string resourceGroup, string addressPrefixes)
    {
        _virtualNetworkName = virtualNetworkName;
        _location = location;
        _resourceGroup = resourceGroup;
        _AddressPrefixes = addressPrefixes;

        var virtualNetwork = new VirtualNetwork("virtualNetwork", new()
        {
            AddressSpace = new Pulumi.AzureNative.Network.Inputs.AddressSpaceArgs
            {
                AddressPrefixes = new[]
            {
                addressPrefixes
            },
            },
            FlowTimeoutInMinutes = 10,
            Location = location,
            ResourceGroupName = resourceGroup,
            VirtualNetworkName = virtualNetworkName,
        });

        // var virtualNetwork = new myVirtualNetwork(virtualNetworkName, location, resourceGroup);
    }
}

public class mySubnet
{
    private readonly string _addressPrefix;
    private readonly string _resourceGroupName;
    private readonly string _subnetName;
    private readonly string _virtualNetworkName;
    private readonly string _resourceName;

    public mySubnet(string resourceName, string addressPrefix, string resourceGroupName, string subnetName, string virtualNetworkName)
    {
        _resourceName = resourceName;
        _addressPrefix = addressPrefix;
        _resourceGroupName = resourceGroupName;
        _subnetName = subnetName;
        _virtualNetworkName = virtualNetworkName;

        var subnet = new Subnet(resourceName, new()
        {
            AddressPrefix = addressPrefix,
            ResourceGroupName = resourceGroupName,
            SubnetName = subnetName,
            VirtualNetworkName = virtualNetworkName
        });
    }
}

public class myNsg
{
    private readonly string _networkSecurityGroupName;
    private readonly string _resourceGroupName;
    private readonly string _location;
    public myNsg(string networkSecurityGroupName, string resourceGroupName, string location)
    {
        _networkSecurityGroupName = networkSecurityGroupName;
        _resourceGroupName = resourceGroupName;
        _location = location;

        var networkSecurityGroupResource = new NetworkSecurityGroup("networkSecurityGroupResource", new()
        {
            ResourceGroupName = resourceGroupName,
            FlushConnection = false,
            Id = "string",
            Location = location,
            NetworkSecurityGroupName = networkSecurityGroupName,
            Tags =
            {
                { "string", "string" },
            },
        });
    }
}

public class myAppServicePlan
{
    private readonly string _location;
    private readonly string _appServicePlanName;
    private readonly string _resourceGroupName;
    private readonly string _resourceName;

    public myAppServicePlan(string location, string appServicePlanName, string resourceGroupName, string resourceName)
    {
        _location = location;
        _appServicePlanName = appServicePlanName;
        _resourceGroupName = resourceGroupName;
        _resourceName = resourceName;

        var appServicePlan = new AppServicePlan(resourceName, new()
        {
            Kind = "app",
            Location = location,
            Name = appServicePlanName,
            ResourceGroupName = resourceGroupName,
            Sku = new Pulumi.AzureNative.Web.Inputs.SkuDescriptionArgs
            {
                Capacity = 1,
                Family = "P",
                Name = "P1",
                Size = "P1",
                Tier = "Premium",
            },
        });

    }
}

//public class myWebApp
//{
//    private readonly string _location;
//    private readonly string _appServicePlanName;
//    private readonly string _resourceGroupName;
//    private readonly string _resourceName;
//    public myWebApp(string location, string resourceGroupName, string appServicePlanName,  string resourceName)
//    {
//        _location=location; 
//        _resourceGroupName = resourceGroupName;
//        _appServicePlanName=appServicePlanName;
//        _resourceName = resourceName;

//        var appServicePlan = AppServicePlan.Get(location, _appServicePlanName);

//        var webApp = new WebApp(appServicePlanName, new()
//        {
//            Kind = "app",
//            Location = location,
//            Name = resourceName,
//            ResourceGroupName = resourceGroupName,
//            ServerFarmId = appServicePlan.Id
//        });
//    }
//}


//public class myFunctionApp
//{
//    private readonly string _location;
//    private readonly string _appServicePlanName;
//    private readonly string _resourceGroupName;
//    private readonly string _resourceName;

//    public myFunctionApp(string location, string appServicePlanName, string resourceGroupName, string resourceName)
//    {
//        _location = location;
//        _appServicePlanName = appServicePlanName;
//        _resourceGroupName = resourceGroupName;
//        _resourceName = resourceName;

//        var webAppFunctionResource = new WebAppFunction(resourceName, new()
//            {
//                Name = "string",
//                ResourceGroupName = resourceGroupName,
//                Kind = "string",
//                Language = "string",
//                FunctionName = "string",
//                Href = "string",
//                InvokeUrlTemplate = "string",
//                IsDisabled = false,
//                Config = "any",
//                FunctionAppId = "string",
//                Files =
//            {
//                { "string", "string" },
//            },
//                    ConfigHref = "string",
//                    ScriptHref = "string",
//                    ScriptRootPathHref = "string",
//                    SecretsFileHref = "string",
//                    TestData = "string",
//                    TestDataHref = "string",
//                });

//    }
//}