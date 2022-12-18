using versoft.asset_manager;
using versoft.data_model;
using versoft.plant.game_logic;
using versoft.save_data;

public class PlayerManager
{
    private PlayerSaveData _playerSaveData;
    private System.Action<int> _onCurrencyModified;

    public void Initialize()
    {
        LoadSave();
    }

    public void AddCurrencyModifiedListener(System.Action<int> onCurrencyModified)
    {
        _onCurrencyModified += onCurrencyModified;
    }

    public void IncreaseCurrency(int currencyIncrease)
    {
        _playerSaveData.SoftCurrency += currencyIncrease;
        _onCurrencyModified?.Invoke(_playerSaveData.SoftCurrency);
    }

    public void LoadSave()
    {
        var saveDataManager = ServiceLocator.Instance.Get<SaveDataManager>();
        if (saveDataManager == null)
        {
            return;
        }

        _playerSaveData = saveDataManager.LoadData<PlayerSaveData>();
        if (_playerSaveData == null)
        {
            _playerSaveData = new PlayerSaveData();
            BuildPlantLayoutSaveData();
        }

        var storeManager = ServiceLocator.Instance.Get<StoreManager>();
        storeManager.RestoreSaveData(_playerSaveData);
        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        plantManager.LoadPlantsFromSaveData(_playerSaveData);
    }

    private void BuildPlantLayoutSaveData()
    {
        if (_playerSaveData == null)
        {
            return;
        }

        _playerSaveData.PlantLayout = new PlantLayoutSaveData();
        _playerSaveData.PlantLayout.Add("default_position", new PlantLayoutElementSaveData(true, 3, ""));
        _playerSaveData.PlantLayout.Add("center_position", new PlantLayoutElementSaveData("default_center_shelf"));
        _playerSaveData.PlantLayout.Add("left_position", new PlantLayoutElementSaveData("default_tall_shelf"));
        _playerSaveData.PlantLayout.Add("right_position", new PlantLayoutElementSaveData("default_tall_shelf"));
    }

    public void SaveToFile()
    {
        var saveDataManager = ServiceLocator.Instance.Get<SaveDataManager>();
        if (saveDataManager == null)
        {
            return;
        }

        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        if (plantManager == null)
        {
            return;
        }

        var storeManager = ServiceLocator.Instance.Get<StoreManager>();
        if (storeManager == null)
        {
            return;
        }

        _playerSaveData.PlantStates = plantManager.GetPlants();
        _playerSaveData.PlantLayout = plantManager.GetLayout();
        _playerSaveData.PurchasesSaveData = storeManager.GetPurchasesSaveData();
        saveDataManager.SaveData(_playerSaveData);
    }

    public int GetSoftCurrency()
    {
        if (_playerSaveData == null)
        { return 0; }

        return _playerSaveData.SoftCurrency;
    }

    public bool CanAfford(int amount)
    {
        return _playerSaveData.SoftCurrency >= amount;
    }

    public bool HasSpaceForPlants()
    {
        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        var layout = plantManager.GetLayout();
        foreach (var (_, layoutSaveData) in layout.Positions)
        {
            if (layoutSaveData.Unlocked && !layoutSaveData.IsFull())
            {
                return true;
            }
        }

        return false;
    }

    public bool CanAcceptPlant(string randomPlant)
    {
        var dataModelDatabase = ServiceLocator.Instance.Get<DataModelDatabase>();
        if (dataModelDatabase == null)
        { return false; }


        return true;
    }

    public void SpendCurrency(int amount)
    {
        _playerSaveData.SoftCurrency -= amount;
    }

    public bool HasUnlockedShelf(string productReference)
    {
        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        var layout = plantManager.GetLayout();
        var layoutSaveElement = layout.Positions[productReference];
        return layoutSaveElement.Unlocked;

    }
}
