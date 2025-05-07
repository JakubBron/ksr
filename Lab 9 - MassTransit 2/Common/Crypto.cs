using System.Text;
using MassTransit.Serialization;

namespace Wspolne;

public class SymmetricKey(byte[]? key, byte[]? iv) : MassTransit.Serialization.SymmetricKey
{
    public byte[]? Key { get; } = key;
    public byte[]? IV { get; } = iv;
}

public class Crypto(string k) : ISymmetricKeyProvider
{
    public bool TryGetKey(string id, out MassTransit.Serialization.SymmetricKey key)
    {
        key = new SymmetricKey(Encoding.ASCII.GetBytes(id)[..32], Encoding.ASCII.GetBytes(k)[..16]);
        return true;
    }
}