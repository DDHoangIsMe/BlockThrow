using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBasePopup : IBasePopup
{
    protected string content;

    public abstract void Show();
    
    public abstract void Hide();
    
    public abstract void SetUpType();
}
