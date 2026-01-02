using UnityEngine;
using UnityEngine.Purchasing;

public enum DiamandPack
{
    SMALL,MEDIUM, BIG, GIANT
};

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;

    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    private const string REMOVE_ADS = "remove_ads";
    private const string SMALL_PACK = "small_pack";
    private const string MEDIUM_PACK = "medium_pack";
    private const string BIG_PACK = "big_pack";
    private const string GIANT_PACK = "giant_pack";

    private const int SMALL_PACK_REWARD = 50;
    private const int MEDIUM_PACK_REWARD = 150;
    private const int BIG_PACK_REWARD = 500;
    private const int GIANT_PACK_REWARD = 1000;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {

        InitializePurchasing();

        CheckAds();
    }

    public bool CheckAds()
    {
        var prod = storeController.products.WithID(REMOVE_ADS);

        return prod != null && prod.hasReceipt;
    }

    private void InitializePurchasing()
    {
        if (IsInit()) return;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(REMOVE_ADS, ProductType.NonConsumable);
        builder.AddProduct(SMALL_PACK, ProductType.Consumable);
        builder.AddProduct(MEDIUM_PACK, ProductType.Consumable);
        builder.AddProduct(BIG_PACK, ProductType.Consumable);
        builder.AddProduct(GIANT_PACK, ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInit()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    public void BuyRemoveAds()
    {
        Debug.Log("buy no ads");
        BuyProduct(REMOVE_ADS);
    }

    public void BuyDiamandPack(DiamandPack type)
    {
        Debug.Log("buy diamand : " + type);
        switch (type)
        {
            case DiamandPack.SMALL:
                BuyProduct(SMALL_PACK);
                break;
            case DiamandPack.MEDIUM:
                BuyProduct(MEDIUM_PACK);
                break;
            case DiamandPack.BIG:
                BuyProduct(BIG_PACK);
                break;
            case DiamandPack.GIANT:
                BuyProduct(GIANT_PACK);
                break;
        }
    }

    private void BuyProduct(string productId)
    {
        if (!IsInit()) return;

        Product product = storeController.products.WithID(productId);

        if(product != null && product.availableToPurchase)
        {
            storeController.InitiatePurchase(product);
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensionProvider)
    {
        storeController = controller;
        storeExtensionProvider = extensionProvider;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP init failed : " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if(args.purchasedProduct.definition.id == REMOVE_ADS)
        {
            //retirer les ads;
            Debug.Log("you have buy no Ads");
        }
        if (args.purchasedProduct.definition.id == SMALL_PACK)
        {
            Stats.Instance.upDiamand(SMALL_PACK_REWARD, true); MainUi.Instance.shopUI.upDiamand();
        }
        if (args.purchasedProduct.definition.id == MEDIUM_PACK)
        {
            Stats.Instance.upDiamand(MEDIUM_PACK_REWARD, true); MainUi.Instance.shopUI.upDiamand();
        }
        if (args.purchasedProduct.definition.id == BIG_PACK)
        {
            Stats.Instance.upDiamand(BIG_PACK_REWARD, true); MainUi.Instance.shopUI.upDiamand();
        }
        if (args.purchasedProduct.definition.id == GIANT_PACK)
        {
            Stats.Instance.upDiamand(GIANT_PACK_REWARD, true); MainUi.Instance.shopUI.upDiamand();
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"Achat échoué : {product.definition.id} ({reason})");
    }

}
