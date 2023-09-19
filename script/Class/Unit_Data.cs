
using System;
using UnityEngine;
[Serializable]
public class Unit_Data
{
    public string name;
    public int Level;
    public Job_Type job;
    public int Hp;
    public int PAttack;
    public int MAttack;
    public int PDef;
    public int MDef;
    public int Agi;
    public int Mov;
    public int Luk;
    public int Lightning;
    public int Ice;
    public int Fire;
    public int Wind;
    public int order;




	// «Øºc¤èªk 
	public Unit_Data(string _name, int _Level, Job_Type _job, int _Hp, int _PAttack,
        int _MAttack, int _PDef, int _MDef, int _Agi, int _Mov, int _Luk,
        int _Lightning, int _Ice, int _Fire,int _Wind)
    {   
        
        this.name = _name;
        this.Level = _Level;
        this.job = _job;
        this.Hp = _Hp;
        this.PAttack = _PAttack;
        this.MAttack = _MAttack;
        this.PDef = _PDef;
        this.MDef = _MDef;
        this.Agi = _Agi;
        this.Mov = _Mov;
        this.Luk = _Luk;
        this.Lightning = _Lightning;
        this.Ice = _Ice;
        this.Fire = _Fire;
        this.Wind = _Wind;
    }
}
