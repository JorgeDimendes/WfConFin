using System.Security.Cryptography;
using System.Text;

namespace WfConFin.Services
{
    public class MD5Hash
    {
        public static string CalcHash(string valor)
        {
			try
			{
				MD5 md5 = MD5.Create();
				byte[] inputBytes = Encoding.ASCII.GetBytes(valor);
				byte[] hash = md5.ComputeHash(inputBytes);
				StringBuilder sb = new StringBuilder();
				for(int i = 0; i < hash.Length; i++)
				{
					sb.Append(hash[i].ToString("X2"));
				}
				return sb.ToString();
			}
			catch (Exception ex)
			{
                //return null;
                throw new ApplicationException("Erro ao calcular o hash MD5", ex);
            }
        }
    }
}
