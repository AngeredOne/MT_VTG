using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class GameSave
{
    public int objkills { get; set; } = 0;
    public int levelsPassed { get; set; } = 0;

    public int shipInd {get; set; } = 0;

    public List<LevelModel> levels = new List<LevelModel>();
}
