#include<windows.h>
#include"klasa3.h"
#include <iostream>


extern volatile int usageCount;


Klasa3::Klasa3() {
	usageCount++;
	};


Klasa3::~Klasa3() {
	usageCount--;
	};


ULONG STDMETHODCALLTYPE Klasa3::AddRef() {
	/*
	Tutaj zaimplementuj dodawanie referencji na obiekt.
	 */

	InterlockedIncrement(&m_ref);	/* str. 7 œrodek z PDFa */
	return m_ref;

	};


ULONG STDMETHODCALLTYPE Klasa3::Release() {
	/*
	Tutaj zaimplementuj usuwania referencji na obiekt.
	Je¿eli nie istnieje ¿adna referencja obiekt powinien zostaæ usuniêty.
	 */
	ULONG rv = InterlockedDecrement(&m_ref);	/* str. 7 œrodek z PDFa */
	if (rv == 0) 
		delete this;
	
	return rv;
	};


HRESULT STDMETHODCALLTYPE Klasa3::QueryInterface(REFIID iid, void **ptr) {
	if(ptr == NULL) return E_POINTER;
	if(IsBadWritePtr(ptr, sizeof(void *))) return E_POINTER;
	*ptr = NULL;
	if(iid == IID_IUnknown) *ptr = this;
	if(iid == IID_IKlasa3) *ptr = this;
	if(*ptr != NULL) { AddRef(); return S_OK; };
	return E_NOINTERFACE;
	};

HRESULT STDMETHODCALLTYPE Klasa3::Test(const char *napis){
	/*
	Tutaj zaimplementuj funkcjê drukuj¹c¹ napis.
	 */

	printf("%s", napis);

	};

