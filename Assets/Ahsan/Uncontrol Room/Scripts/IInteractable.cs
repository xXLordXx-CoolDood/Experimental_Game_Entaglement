using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public float Value { get; set; }

    public bool IsOutlined { get; set; }

    void ToggleOutline(bool value);
    void Interact();
}
