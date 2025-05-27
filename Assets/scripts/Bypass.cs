using UnityEngine.Networking;

public class BypassCertificate : CertificateHandler
{
    // Este CertificateHandler desactiva la validaci�n SSL (solo para desarrollo local)
        protected override bool ValidateCertificate(byte[] certificateData) => true;
}
