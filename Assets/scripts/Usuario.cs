using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Usuario
{
    public int id;
    public string name;
    public float height;
    public string photoUrl;
    public int puntosTotales = 0;
    public int puntosSesion = 0;
    public List<Logro> logros = new List<Logro>();
    public string fechaRegistro;
    public int numeroSesiones = 0;
    public float tiempoTotalEjercicio = 0f;

    public Usuario(string nombre, float altura)
    {
        this.name = nombre;
        this.height = altura;
    }
}

[Serializable]
public class UsuarioList
{
    public List<Usuario> usuarios;
}