#include<windows.h>

class KlasaFactory: public IClassFactory {
public:	
	KlasaFactory();
	~KlasaFactory();
	virtual ULONG STDMETHODCALLTYPE AddRef();
	virtual ULONG STDMETHODCALLTYPE Release();
	virtual HRESULT STDMETHODCALLTYPE QueryInterface(REFIID id, void **rv);
	virtual HRESULT STDMETHODCALLTYPE LockServer(BOOL v);
	virtual HRESULT STDMETHODCALLTYPE CreateInstance(IUnknown *outer, REFIID id, void **rv);

private:	

	/*
	Tutaj zdeklaruj zmienne:
		- licznik blokad,
		- licznik referencji.
	 */

	volatile ULONG m_ref;			/* wype�nij jak porpzednio */
	volatile ULONG m_lock;			/* typ jak wy�ej */

	};
