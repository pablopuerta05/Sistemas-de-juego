// IFloatingTextService.cs
using UnityEngine;

public interface IFloatingTextService
{
    void ShowFloatingText(string text, Transform target, float duration = 1f, float speed = 50f);
}
