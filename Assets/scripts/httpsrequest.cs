using UnityEngine;

using System;
using System.Net;
using System.IO;

public class httpsrequest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Llamar a la función para obtener datos HTTPS
        getHttpsData();

    }

    public void getHttpsData()
    {
        try
        {
            string url = "https://192.168.1.252:3000/api/usuarios";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true; // Ignorar errores de certificado

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                string responseText = reader.ReadToEnd();

                Debug.Log("Response: " + responseText);

            }
            else
            {
                Console.WriteLine("Error 400: " + response.StatusCode);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}
