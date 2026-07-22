using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;


[System.Serializable]
public struct Damage : INetworkSerializable
{
    public float physical;
    public float magic;
    public float fire;
    public float holy;
    public float lightning;
    public float poise;

    public Damage(float physical = 0, float magic = 0, float fire = 0, float holy = 0, float lightning = 0, float poise = 0)
    {
        this.physical = physical;
        this.magic = magic;
        this.fire = fire;
        this.holy = holy;
        this.lightning = lightning;
        this.poise = poise;
    }

    public readonly float TotalDamage => physical + magic + fire + holy + lightning;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref physical);
        serializer.SerializeValue(ref magic);
        serializer.SerializeValue(ref fire);
        serializer.SerializeValue(ref holy);
        serializer.SerializeValue(ref lightning);
        serializer.SerializeValue(ref poise);
    }

    public static Damage operator +(Damage a, Damage b)
    {
        return new Damage(
            a.physical + b.physical, 
            a.magic + b.magic, 
            a.fire + b.fire, 
            a.holy + b.holy, 
            a.lightning + b.lightning, 
            a.poise + b.poise);
    }

    public static Damage operator -(Damage a, Damage b)
    {
        // Damage reduction but not below 0
        return new Damage(
            Math.Max(0, a.physical - b.physical), 
            Math.Max(0, a.magic - b.magic), 
            Math.Max(0, a.fire - b.fire), 
            Math.Max(0, a.holy - b.holy), 
            Math.Max(0, a.lightning - b.lightning), 
            Math.Max(0, a.poise - b.poise));
    }

    public static Damage operator *(Damage a, float b)
    {
        return new Damage(
            b * a.physical, 
            b * a.magic, 
            b * a.fire, 
            b * a.holy, 
            b * a.lightning, 
            b * a.poise);
    }
}