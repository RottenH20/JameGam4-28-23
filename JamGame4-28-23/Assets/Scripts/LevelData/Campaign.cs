using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCampaign", menuName = "HammerClimb/Campaign")]
public class Campaign : ScriptableObject {
    public string displayName = "New Campaign";
    public LevelData[] levels;
}
