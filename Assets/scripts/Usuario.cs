using System;
using System.Collections.Generic;

[Serializable]
public class Usuario
{
    public int id;
    public string name;
    public float height;
    public string photoUrl;
    public int puntosTotales = 0;
    public int puntosSesion = 0;
    public List<string> logros = new List<string>();
    public List<string> retosCompletados = new List<string>();
    public DateTime fechaRegistro;
    public int numeroSesiones = 0;
    public float tiempoTotalEjercicio = 0f;

    public Usuario(string nombre, float altura)
    {
        this.name = nombre;
        this.height = altura;
        this.fechaRegistro = DateTime.Now;
    }
}


[Serializable]
public class UsuarioList
{
    public List<Usuario> usuarios;
}