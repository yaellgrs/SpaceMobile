using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.PointerEventData;


[CreateAssetMenu(menuName = "Game/Quest")]
public class  Quests : ScriptableObject
{
    [Tooltip("Le numero de la quête en gros ( on commence par level == 1 puis level ==2 etc )")]
    public int level;
    [Tooltip("Type de la quête pour lier l'objectif, si jamais y'a pas celui que tu veux tu me le dis")]
    public QuestType type;

    [Header("-- Objectifs --")]
    [Tooltip("object a atteindre selon le type pour accomplir la quête, si jamais tu veux pas t'embeter avec avec les puissance de 10, met le chiffre en Mantisse avec Exp = 0.")]
    public BigNumber objectif;

    [Header("-- Rewards --")]
    [Tooltip("Nombre de diamants obtenu avec la quête")]
    public int rewardDiamand = 2;

    [Header("-- Text --")]
    [TextArea(20, 2)]
    public string text = "Met le text ici puis dis le moi, je le mettrais dans la table de traduction.";
}
