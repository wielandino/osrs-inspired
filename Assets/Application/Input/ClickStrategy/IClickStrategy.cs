using System.Collections.Generic;
using UnityEngine;

public interface IClickStrategy
{
    public bool CanHandle(RaycastHit hit);

    public void Handle(RaycastHit hit);
    
    public List<ContextMenuOption> GetContextMenuOptions(RaycastHit hit);

    public int Priority => 0;
}