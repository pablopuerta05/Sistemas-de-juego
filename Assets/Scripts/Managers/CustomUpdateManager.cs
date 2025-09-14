using System.Collections.Generic;
using UnityEngine;

// Este es el �nico MonoBehaviour que contiene un Update() real.
// Se encarga de llamar al m�todo Tick() de todas las clases registradas.
// As� evitamos tener Update() en muchos scripts.

public class CustomUpdateManager : MonoBehaviour
{
    // Lista de todas las clases que deben recibir el Tick()
    private List<IUpdatable> updatables = new List<IUpdatable>();

    // Unity llama a Update() autom�ticamente. Nosotros lo usamos para controlar la l�gica del juego manualmente.
    void Update()
    {
        foreach (var u in updatables)
        {
            u.Tick(Time.deltaTime); // Llamamos al m�todo Tick de cada clase registrada
        }
    }

    // M�todo para registrar una clase al sistema de actualizaci�n
    public void Register(IUpdatable updatable)
    {
        if (!updatables.Contains(updatable))
            updatables.Add(updatable);
    }

    // M�todo para quitar una clase del sistema si ya no necesita actualizarse
    public void Unregister(IUpdatable updatable)
    {
        updatables.Remove(updatable);
    }
}