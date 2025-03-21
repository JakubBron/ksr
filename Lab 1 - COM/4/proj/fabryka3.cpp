#include "fabryka3.h"
#include "klasa3.h"

extern volatile int usageCount;

KlasaFactory::KlasaFactory() {
	/*
	Tutaj dodaj inicjalizacj� warto�ci zmiennych licznik�w referencji i blokad.
	 */
	usageCount++;
	m_ref = 0;				/* str. 7 d� z PDFa */
	m_lock = 0;
	
	};
	

KlasaFactory::~KlasaFactory() {
	usageCount--;
	};

	
ULONG STDMETHODCALLTYPE KlasaFactory::AddRef() {
	/*
	Tutaj zaimplementuj dodawanie referencji na obiekt.
	 */

	InterlockedIncrement(&m_ref);	/* str. 7 �rodek z PDFa */
	return m_ref;

	};


ULONG STDMETHODCALLTYPE KlasaFactory::Release() {
	/*
	Tutaj zaimplementuj usuwania referencji na obiekt.
	Je�eli nie istnieje �adna referencja obiekt powinien zosta� usuni�ty.
	 */

	ULONG rv = InterlockedDecrement(&m_ref);	/* str. 7 �rodek z PDFa */
	if (rv == 0)
		delete this;

	return rv;

	};


HRESULT STDMETHODCALLTYPE KlasaFactory::QueryInterface(REFIID id, void **ptr) {
	if(ptr == NULL) return E_POINTER;
	if(IsBadWritePtr(ptr, sizeof(void *))) return E_POINTER;
	*ptr = NULL;
	if(id == IID_IUnknown) *ptr = this;
	if(id == IID_IClassFactory) *ptr = this;
	if(*ptr != NULL) { AddRef(); return S_OK; };
	return E_NOINTERFACE;
	};


HRESULT STDMETHODCALLTYPE KlasaFactory::LockServer(BOOL v) {
	if(v) m_lock++; 
	else m_lock--;
	return S_OK;
	};


HRESULT STDMETHODCALLTYPE KlasaFactory::CreateInstance(IUnknown *outer, REFIID id, void **ptr) {
	Klasa3 *k;
	if(ptr == NULL) return E_POINTER;
	if(IsBadWritePtr(ptr, sizeof(void *))) return E_POINTER;
	*ptr = NULL;
	if(id == IID_IUnknown || id == IID_IKlasa3) {
		k = new Klasa3();
		if(k == NULL) return E_OUTOFMEMORY;
		k->AddRef();
		*ptr = k;
		return S_OK;
		};
	return E_NOINTERFACE;
	};
