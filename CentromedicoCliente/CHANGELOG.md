# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

## 0.1.0 (2021-11-01)


### ⚠ BREAKING CHANGES

* se cambia el método register del controlador account hacia el controlador token
* se cambia el nombre del enpoint getAvailableTimeList por getHoursList
* se cambia el nombre del endpoint getNewCita por getCitaForm
* se eliminar el método getCoberturas del controlador citas
* se devuelve un exception personalizada cuando la petición es erronea o el servidor falla
* se retorna una variable con el nombre de error el cual es una array de string con los mensajes de las exceptiones producidas en la aplicación

* se implementa el patrón repository-service non-generic en la api ([dec6a06](https://github.com/KevinJ0/centromedico_cliente/commit/dec6a06acb39bd1d01ee15776c57d1657f62c306))
