using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGamePlayController
{
    void Init(string configName);

    void UnInit();
}
