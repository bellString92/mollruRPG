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

    public int SelNum { get=>_selNum; private set=>_selNum = value; }
    public string JobSelect { get => _jobSelect; private set => _jobSelect = value; }
    public string NickName { get => _nickName; private set => _nickName = value; }

    public CharacterData(int selNum, string jobSelect, string nickName)
    {
        SelNum = selNum;
        JobSelect = jobSelect;
        NickName = nickName;
    }

}
