﻿using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using System.Collections.Generic;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

return await Pulumi.Deployment.RunAsync(() =>
{
    const string __resourceGroup = "rg-demo";
    const string __location = "UKSouth";
    const string __vnet = "Vnet2";
    const string __accountName = "demotestdb001";

    var resourceGroup = new myResourceGroup("myResourceGroup", __resourceGroup, __location);
    var storageAccount = new myStorageAccount("sa002qw", __resourceGroup, __location);

    var virtualNetwork = new myVirtualNetwork(__vnet, __location, __resourceGroup, "10.10.100.0/24");
    var subnet1 = new mySubnet("subnet1", "10.10.100.32/27", __resourceGroup, "subnet1", __vnet);
    var subnet2 = new mySubnet("subnet2", "10.10.100.64/27", __resourceGroup, "subnet2", __vnet);

    var nsg = new myNsg("testNsg", __resourceGroup, __location);
    var webAppServicePlan = new myAppServicePlan(__location, "webAppServicePlan", __resourceGroup, "webAppServicePlan", "app");

    //var cosmosDb = new myAzureCosmosDb(__location, __resourceGroup, __accountName);
    var manageIdentity1 = new myManagedIdentity(__location, __resourceGroup, "user1-mi", "userAssignedIdentity1");
    var manageIdentity2 = new myManagedIdentity(__location, __resourceGroup, "user2-mi", "userAssignedIdentity2");

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
    private readonly string _resourceGroup;
    private readonly string _location;

    public myStorageAccount(string name, string resourceGroup, string location)
    {
        _name = name;
        _resourceGroup = resourceGroup;
        _location = location;

        var resourceGroupResource = new ResourceGroup(resourceGroup, new()
        {  
            ResourceGroupName = resourceGroup,
            Location = location,
        });

        var storageAccountArgs = new StorageAccountArgs
        {
            AccountName = name,
            ResourceGroupName = resourceGroupResource.Name,
            Location = resourceGroupResource.Location,
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
    private readonly string _kind;

    public myAppServicePlan(string location, string appServicePlanName, string resourceGroupName, string resourceName, string kind)
    {
        _location = location;
        _appServicePlanName = appServicePlanName;
        _resourceGroupName = resourceGroupName;
        _resourceName = resourceName;
        _kind = kind;

        var appServicePlan = new AppServicePlan(resourceName, new()
        {
            Kind = kind,
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

        var webApp = new WebApp(kind, new()
        {
            Location = location,
            Name = resourceName,
            ResourceGroupName = resourceGroupName,
            ServerFarmId = appServicePlan.Id
        });
    }
}

public class myAzureCosmosDb
{
    private readonly string _location;
    private readonly string _resourceGroupName;
    private readonly string _accountName;

    public myAzureCosmosDb(string location, string resourceGroupName, string accountName)
    {
        _location = location;
        _resourceGroupName = resourceGroupName;
        _accountName = accountName;

        var databaseAccount = new Pulumi.AzureNative.DocumentDB.DatabaseAccount("databaseAccount", new()
        {
            AccountName = accountName,
            CreateMode = Pulumi.AzureNative.DocumentDB.CreateMode.Default,
            DatabaseAccountOfferType = Pulumi.AzureNative.DocumentDB.DatabaseAccountOfferType.Standard,
            Location = location,
            Locations = new[]
               {
                    new Pulumi.AzureNative.DocumentDB.Inputs.LocationArgs
                    {
                        FailoverPriority = 0,
                        IsZoneRedundant = false,
                        LocationName = location,
                    },
                },
            ResourceGroupName = resourceGroupName,
            //VirtualNetworkRules = new[]
            //{
            //    new Pulumi.AzureNative.DocumentDB.Inputs.VirtualNetworkRuleArgs
            //    {
            //        Id = "string",
            //        IgnoreMissingVNetServiceEndpoint = false,
            //    }
            //}
        });
    }
}

public class myManagedIdentity
{
    private readonly string _location;
    private readonly string _resourceGroupName;
    private readonly string _resourceName;
    private readonly string _xxx;


    public myManagedIdentity(string location, string resourceGroupName, string resourceName, string xxx)
    {
        _location = location;
        _resourceGroupName = resourceGroupName;
        _resourceName = resourceName;
        _xxx = xxx;

        var userAssignedIdentity = new Pulumi.AzureNative.ManagedIdentity.UserAssignedIdentity(xxx, new()
        {
            Location = location,
            ResourceGroupName = resourceGroupName,
            ResourceName = resourceName,
            Tags =
            {
                { "key1", "value1" },
                { "key2", "value2" },
            }
        });
    }
}

