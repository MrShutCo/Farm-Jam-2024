using UnityEngine;
using System.Collections;
using Assets.Script.Humans;

public class Package : MonoBehaviour
{
	public EResource Resource { get; private set; }
	public int Amount { get; private set; }

	public void SetPackage(EResource _resource, int _amount)
	{
		Resource = _resource;
		Amount = _amount;
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

    public void Use()
    {
		Destroy(gameObject);
    }
}

