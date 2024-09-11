using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset(AssetPath = "Resources")]
public class ItemData : ScriptableObject
{
	public List<ItemEntity> Entities;
}
