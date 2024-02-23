using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWorld
{
    Wild, Home
}
public class World : MonoBehaviour
{
    public EWorld WorldType;
    public Grid2D Grid;
    
}
