using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData
{
    private int _selNum;
    private string _jobSelect;
    private string _nickName;
    private BattleStat _myStat;

    public int SelNum { get=>_selNum; private set=>_selNum = value; }
    public string JobSelect { get => _jobSelect; private set => _jobSelect = value; }
    public string NickName { get => _nickName; private set => _nickName = value; }

    public BattleStat MyStat { get => _myStat; set => _myStat = value; }

    public CharacterData()
    {
        _myStat = new OriBattleStat().oriBattleStat;
    }

    public CharacterData(int selNum, string jobSelect, string nickName) : this()
    {
        SelNum = selNum;
        JobSelect = jobSelect;
        NickName = nickName;
    }


}
