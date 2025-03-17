# AccelByte Unity SDK Google

Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
This is licensed software from AccelByte Inc, for limitations
and restrictions contact your company contract manager.

# Overview
Unity SDK Google is an extension package to enable Accelbyte SDK support for Android Google Authentication and Authorization.

## Prerequisites
1. [AccelByte Unity SDK](https://github.com/AccelByte/accelbyte-unity-sdk) minimum version: 17.6.0. For more information about configuring AccelByte Unity SDK, see [Install and configure the SDK](https://docs.accelbyte.io/gaming-services/getting-started/setup-game-sdk/unity-sdk/#install-and-configure).

2. Android minimum API level 30. 

## Dependencies
Unity SDK Google requires following additional configuration:

1. Android Credentials Manager library.
   Android Credentials Manager provides unified access to a user's credentials to provide seamless and secure sign-in experiences. Required configuration will be included in `Assets\Plugins\Android\gradleTemplate.properties` and you could import it from `AccelByteUnitySdkGoogle.unitypackage`.
2. Customized Unity Activity.
    `AccelByteUnitySdkGoogle.unitypackage` will provide `AccelBytePlayerActivity.java` extending `UnityPlayerActivity` to interact with the Google login activity result and customized `AndroidManifest.xml` to set the `AccelBytePlayerActivity.java` as the application entry point.

## How to Install
1. In your Unity project, go to `Window > Package Manager`.
2. Click the + icon on the Package Manager window and click `Add package from git URL...`
3. Paste the following link into the URL field and click Add: `https://github.com/AccelByte/accelbyte-unity-sdk-google.git`.
4. Install `_Install/AccelByteUnitySdkGoogle.unitypackage` to your game directory. Import all of the item if there's no conflict with your Android build configuration files.
5. If there is any conflict with your current Android build configuration, please follow these guide how to merge them:
    - If you're using custom main activity extending `UnityPlayerActivity`, ignore `AndroidManifest.xml` from the package, and add the following code to `YourCustomUnityPlayerActivity` to allow AccelByte SDK get the login activity result when requesting additional scopes:

        ```java
        import com.accelbyte.googlesigninlib.AccelByteGoogleSignIn;
        import static com.accelbyte.googlesigninlib.AccelByteConst.REQUEST_AUTHORIZE;
        
        public class YourCustomUnityPlayerActivity extends UnityPlayerActivity
        {
            protected void onActivityResult(int requestCode, int resultCode, Intent data)
            {
                super.onActivityResult(requestCode, resultCode, data);
                
                if (requestCode == REQUEST_AUTHORIZE)
                {
                    AccelByteGoogleSignIn.getInstance().onActivityResult(resultCode, data, this);
                }
            }
        }
        ```

    - If your'e enabling custom main gradle template, ignore `mainTemplate.gradle` from the package, and add the following configuration to your `Assets\Plugins\Android\mainTemplate.gradle` to enable androidX library.

        ```groovy
        **ADDITIONAL_PROPERTIES**
        android.useAndroidX=true
        android.enableJetifier=true
        ```

    - If you're enabling Custom Gradle Properties Template, ignore `gradleTemplate.properties` from the package, and add the following configuration to your `Assets\Plugins\Android\gradleTemplate.properties` to add required libraries into your project.
        ```groovy
        apply plugin: 'com.android.library'
        **APPLY_PLUGINS**

        dependencies {
            implementation fileTree(dir: 'libs', include: ['*.jar'])
            implementation 'androidx.credentials:credentials:1.3.0'
            implementation 'androidx.credentials:credentials-play-services-auth:1.3.0'
            implementation 'com.google.android.libraries.identity.googleid:googleid:1.1.1'
            implementation 'com.google.android.gms:play-services-auth:16.0.1'
        **DEPS**}
        ```
    - If you're using `Unity 6`, ignore `AndroidManifest.xml` and `AccelBytePlayerActivity.java` from the package. Requesting additional scopes is still under development on `Unity 6`.

6. Add assembly reference of `com.AccelByte.GoogleExtension` to your project.

# Limitations
- Requesting additional scopes on `Unity 6` is still under development.

# Features Usage

## Sign In with Google

We provide easier way to let player perform Sign in With Google platform. Therefore player doesn't need to register a new account to AGS to utilize the AGS features.

### Add Google Web Cient Id Configuration
To get the client ID for your game from Google Cloud, follow [this documentation](https://cloud.google.com/endpoints/docs/frameworks/java/creating-client-ids).
1. Open AccelByte Client Configuration on `AccelByte > Edit Client Settings`.
2. Find `Google Configs` section and fill `Google Web Client Id` field using generated Google Client Id.

### Code Implementation

1. Header Initialization
```csharp
using AccelByte.Core;
using AccelByte.Models;
```

1. Get Google Id Token and Server Auth Code
```csharp
private string googleIdToken;
private string googleAuthCode;
private AccelByte.ThirdParties.Google.AccelByteGoogle accelByteGoogle;

void Start()
{
    AGSInitialize();
}

void AGSInitialize()
{
    accelByteGoogle = new AccelByte.ThirdParties.Google.AccelByteGoogle();
}

void GetSignInWithGoogle()
{
    var optionalParams = new AccelByte.ThirdParties.Google.GetSignInWithGoogleOptionalParameters()
    {
        RequestServerAuthCode = true,
        Scopes = new string[]
        {
            "profile",
            "email"
        }
    };

    accelByteGoogle.GetSignInWithGoogle(optionalParams)
        .OnSuccess((result) =>
        {
            googleIdToken = result.IdToken;
            googleAuthCode = result.ServerAuthCode;
            UnityEngine.Debug.Log("Sign in with Google succeeded");
        })
        .OnFailed((error) =>
        {
            UnityEngine.Debug.LogWarning($"Sign in with Google failed: {error.Message}");
        });
}
```

3. Login to AGS
```csharp
private void AGSLogin()
{
    if (!string.IsNullOrEmpty(googleIdToken))
    {
        var optionalParams = new LoginWithOtherPlatformV4OptionalParameters()
        {
            CreateHeadless = false
        };

        AccelByteSDK.GetClientRegistry().GetApi().GetUser().LoginWithOtherPlatformV4(
            new Models.LoginPlatformType(AccelByte.Models.PlatformType.Google)
            , googleIdToken
            , optionalParams
            , result =>
            {
                if (result.IsError)
                {
                    UnityEngine.Debug.LogWarning($"Login with AGS failed [{result.Error.error}]: {result.Error.error_description}");
                    return;
                }

                UnityEngine.Debug.Log("Login with AGS succeeded");
            });
    }
}
```
The full script on the package sample named `Sign in with Google`.

## In-App Purchasing

### Overview
There are three kinds of `ProductType` based on Unity Documentation. Previously we supported synchronizing consumable, non-consumable entitlements with the AccelByte server. And now also supports subscriptions as well.

### Configure Your Game

> Please contact AccelByte support for guideline document

### Prerequisites

1. Import package [UnityPurchasing](https://docs.unity3d.com/Packages/com.unity.purchasing@4.12/manual/index.html) library to the project.
2. Please refers to official [Unity documentation](https://docs.unity3d.com/Manual/UnityIAPSettingUp.html) on how to install it.
3. Open `File > Build Settings > Player Settings`. On `Player` categories, navigate to `Android Settings` build section. Please check `Custom main gradle template` and `Custom Gradle Properties Template` to avoid duplicated class exception.
> **Important** : Ensure that the player is already **Logged in** using AGS service before using this feature. You may check player's login status by using this snippet `bool isLoggedIn = AccelByteSDK.GetClientRegistry().GetApi().GetUser().Session.IsValid();` and isLoggedIn should return `true`.

### Code Implementation

1. Sign in With Google, please refer to [previous part](https://github.com/AccelByte/accelbyte-unity-sdk-google?tab=readme-ov-file#sign-in-with-google)
2. Please create `MonoBehavior` class implementing `IDetailedStoreListener`. Unity IAP will handle the purchase and trigger callbacks using this interface. Then prepare the following variables.
```csharp
IStoreController storeController;

public Button BuyGoldButton;
public Button BuyWeaponButton;
public Button BuySeasonPassButton;

private string goldProductId = "item_gold"; // assume that the registered consumable product id is named Item_gold
private ProductType goldProductType = ProductType.Consumable;
private string weaponProductId = "item_weapon"; // assume that the registered non-consumable product id is named item_weapon
private ProductType weaponProductType = ProductType.NonConsumable;
private string seasonPassProductId = "item_season_pass"; // assume that the registered subscription product id is named item_season_pass
private ProductType seasonPassProductType = ProductType.Subscription;
```
3. Prepare three [Buttons](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/script-Button.html) to login and trigger the purchasing event. Using Unity Editor's inspector, attach those buttons into `public Button BuyGoldButton;`, `public Button BuyWeaponButton;`, and `public Button BuySeasonPassButton;`.
4. Initialize Purchasing.
```csharp
void Start()
{
    InitializePurchasing();
}

void InitializePurchasing()
{
    var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

    if (!string.IsNullOrEmpty(user.Session.UserId))
    {
        var uid = System.Guid.Parse(user.Session.UserId);
        builder.Configure<IGooglePlayConfiguration>().SetObfuscatedAccountId(uid.ToString().Replace("-",""));
    }

    //Add products that will be purchasable and indicate its type.
    builder.AddProduct(GoldProductId, GoldProductType);
    builder.AddProduct(WeaponProductId, WeaponProductType);
    builder.AddProduct(SeasonPassProductId, SeasonPassProductType);

    UnityPurchasing.Initialize(this, builder);
}

public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
{
    Debug.Log("In-App Purchasing successfully initialized");
    storeController = controller;
}
```

5. Prepare several functions that will be trigger the purchasing event
```csharp
private void BuyGold()
{
    storeController.InitiatePurchase(productId);
}

private void BuyWeapon()
{
    storeController.InitiatePurchase(weaponProductId);
}

private void BuySeasonPass()
{
    storeController.InitiatePurchase(seasonPassProductId);
}

```
6. Assign each buttons
```csharp
void Start()
{
    ButtonAssigning();
}

void ButtonAssigning()
{
    BuyGoldButton.onClick.AddListener(BuyGold);
    BuyWeaponButton.onClick.AddListener(BuyWeapon);
    BuySeasonPassButton.onClick.AddListener(BuySeasonPass);
}
```
7. Handle Process Purchase. Please note that it **must** return `PurchaseProcessingResult.Pending` because purchased item will be synchronized with AccelByte's Backend. [reference](https://docs.unity3d.com/2021.3/Documentation/Manual/UnityIAPProcessingPurchases.html). If client successfully purchase item from Google Play Store, `ProcessPurchase` will be triggered, else `OnPurchaseFailed` will be triggered.
```csharp
public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
{
    var product = purchaseEvent.purchasedProduct;

    Debug.Log($"Purchase Complete - Product: {product.definition.id}");
    if (product.definition.type == ProductType.Subscription)
    {
        AGSSubscriptionEntitlementSync(product);
    }
    else
    {
        AGSEntitlementSync(product);
    }

    return PurchaseProcessingResult.Pending;
}

public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
{
    Debug.LogError($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
}
```

8. Sync Purchased Product with AGS
```csharp

private void AGSEntitlementSync(Product product)
{

    // Please note that Sync will work after the player is logged in using AB service
    // Please refer to https://github.com/AccelByte/accelbyte-unity-sdk-google-play?tab=readme-ov-file#sign-in-with-google-play-games for implementation
    try
    {
        string receiptPayload = JObject.Parse(product.receipt)["Payload"].ToString();
        var receiptJson = JObject.Parse(receiptPayload)["json"].ToString();
        var receiptObject = JObject.Parse(receiptJson);

        var orderId = ((string)receiptObject["orderId"]);
        var packageName = ((string)receiptObject["packageName"]);
        var productId = ((string)receiptObject["productId"]);
        var purchaseTime = ((long)receiptObject["purchaseTime"]);
        var purchaseToken = ((string)receiptObject["purchaseToken"]);
        var autoAck = product.definition.type == ProductType.NonConsumable;

        entitlement.SyncMobilePlatformPurchaseGoogle(
                orderId
                , packageName
                , productId
                , purchaseTime
                , purchaseToken
                , autoAck
                , syncResult =>
                {
                    if (syncResult.IsError)
                    {
                        UnityEngine.Debug.LogWarning(syncResult.Error.Message);
                        return;
                    }

                    if (syncResult.Value.NeedConsume)
                    {
                        FinalizePurchase(product);
                    }
                });
        }
    }
    catch (Exception e)
    {
        UnityEngine.Debug.LogWarning($"Failed to sync with AB {e.Message}");
    }

}

private void AGSSubscriptionEntitlementSync(Product purchasedSubscription)
{
    // Please note that Sync will work after the player is logged in using AB service
    try
    {
        string receiptPayload = JObject.Parse(product.receipt)["Payload"].ToString();
        var receiptJson = JObject.Parse(receiptPayload)["json"].ToString();
        var receiptObject = JObject.Parse(receiptJson);

        var orderId = ((string)receiptObject["orderId"]);
        var packageName = ((string)receiptObject["packageName"]);
        var productId = ((string)receiptObject["productId"]);
        var purchaseTime = ((long)receiptObject["purchaseTime"]);
        var purchaseToken = ((string)receiptObject["purchaseToken"]);
        var autoAck = true;
            entitlement.SyncMobilePlatformSubscriptionGoogle(
                orderId
                , packageName
                , productId
                , purchaseTime
                , purchaseToken
                , autoAck
                , syncResult =>
                {
                    if (syncResult.IsError)
                    {
                        UnityEngine.Debug.LogWarning(syncResult.Error.Message);
                        return;
                    }

                    if (syncResult.Value.NeedConsume)
                    {
                        FinalizePurchase(product);
                    }
                });
        }
}

```
> Please note that when syncing subscription product, if `autoAck` set to `false`, Game client need to manual ack it, otherwise this transaction will be expired. If `true`, game client don’t need to do anything.

9. Finalize Pending Purchase
```csharp
private void FinalizePurchase(Product purchasedProduct)
{
    Debug.Log($"Confirm Pending Purchase for: {purchasedProduct.definition.id}");
    storeController.ConfirmPendingPurchase(purchasedProduct);
}
```
The full script on the package sample named `In App Purchase`.