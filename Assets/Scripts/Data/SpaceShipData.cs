using System;
using System.Globalization;
using UnityEngine;

public enum SpaceShipType { Basic, Fire, Ice };

[Serializable]
public class SpaceShipDico //pour pouvoir serialiser le dictionnaire
{
    public SpaceShipType type;
    public SpaceShipData data;
}

[System.Serializable]
public class SpaceShipData
{
/*    public BigNumber BN_money_basic; // iron, 
    public BigNumber BN_money_advanced; // uranium, 
    public BigNumber BN_starParticle;*/

    public int level = 1;
    //levels des spaceships

    //upgrades ( List<Upgrades>
    //machine ? 

    public SpaceShipData()
    {

    }
}

/*
 Stats

enum spaceShiptype;

spaceShiptype curentShip( enum ? ) 
Dictionnaire<spaceShiptype, SpaceShipData>{
	//monnaie des spaceShip 
	//levels des spaceships

	//upgrades ( List<Upgrades> ) -> stocker les ~données dans des scriptable objects
	//machine ? 
}


SpaceShipManager

*setIcons
 */