# PE_Scrapping
Pequeña aplicación para obtener datos de las elecciones presidenciales de Perú, que es información pública y se encuentra publicada en la misma página de la ONPE.

Esta aplicación se encuentra en versión de pruebas. Si alguien quiere contribuir y mejorar, es bienvenido.

Utiliza ChromeDriver versión 91. Verificar que chromedriver.exe se encuentre en la carpeta "runtimes", en la raiz de la aplicación.
En el repositorio se encuentra en la carpeta "chromedriver_win32".

Varias versiones de ChromeDriver, por si hay errores con la versión de Chrome:<br />
http://chromedriver.storage.googleapis.com/index.html

APIs utilizadas<br />
1ra Vuelta<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/ecp/ubigeos/T<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/locales/010202<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/actas/11/010202/0032<br />
Request URL: https://resultadoshistorico.onpe.gob.pe/v1/EG2021/mesas/detalle/000169<br />
<br />
2da Vuelta<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/ecp/ubigeos/T<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/mesas/locales/020206<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/mesas/actas/11/020206/I924<br />
Request URL: https://api.resultadossep.eleccionesgenerales2021.pe/mesas/detalle/060486<br />