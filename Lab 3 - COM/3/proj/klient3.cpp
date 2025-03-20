#include<windows.h>
#include<iostream>
#import "klasa.tlb" no_namespace

int main() {

	CoInitializeEx(NULL, COINIT_MULTITHREADED);
	IKlasa *s;
	HRESULT rv;
	rv = CoCreateInstance(__uuidof(Klasa), NULL, CLSCTX_ALL, __uuidof(IKlasa), (void **)&s);
	if (SUCCEEDED(rv)) {
		s->Test("Testowanie, zadanie 3 ok!");
		s->Release();
	}
	else {
		std::cout << "Nie dziala! Prawdopodobnie klasa jest niezarejestrowana";
	}
	
	CoUninitialize();

	return 0;
};
