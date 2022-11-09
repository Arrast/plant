using System.Collections.Generic;
using System.Collections;

public partial class PlantModel 
{
	public string Id;
	public string ModelId;
	public Rarity Rarity;
	public string[] Stages;
	public float[] LevelUpTimes;
	public int[] LevelUpRewards;
	public float Light;
	public float Food;
	public float Water;
	public CoinIncrementData TimeToEarnCoin;
}