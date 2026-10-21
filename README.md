PokeAPI .NET para Scisa

Aplicación web desarrollada en **.NET Core 8** con arquitectura MVC, que consume la **PokeAPI** para mostrar un catálogo interactivo de Pokémon con filtros, paginación manual por servidor, modal de detalles, exportación a Excel y envío de correos electrónicos.

---

## Características principales
* **Listado y Grid:** Visualización tradicional en tabla con diseño adaptativo y Bootstrap 5.
* **Filtros Combinados:** Filtrado por nombre y por especie de Pokémon utilizando LINQ.
* **Paginación Manual:** Implementada del lado del servidor.
* **Detalle de Pokémon:** Modal asincrónico.
* **Exportación a Excel:** Generación de reportes dinámicos de los elementos filtrados usando **ClosedXML**.
* **Envío de Correos:** Soporte para envío individual (por Pokémon) o general (catálogo filtrado) en formato HTML mediante **MailKit**.
* **Optimización con Caché:** Implementación de `IMemoryCache` para almacenar localmente las especies y evitar llamadas repetitivas a la API.

---

## Configuración y Ejecución

1. Clonar el repositorio:
    ```json
    git clone [https://github.com/Maxter-San/ScisaPokeApi](https://github.com/Maxter-San/ScisaPokeApi)

2. Abrir el proyecto en Visual Studio Code o tu IDE preferido.

3. Restaurar las dependencias e iniciar el proyecto:
    ```json
    dotnet restore
    dotnet run

---

## Guía para probar el envío de correos

Para que el envío de correos funcione correctamente, es necesario configurar las credenciales SMTP de tu cuenta de correo en el archivo `appsettings.json`.

1. Abre el archivo appsettings.json en la raíz del proyecto.

2. Localiza la sección "EmailSettings":
    ```json
    "EmailSettings": {
        "Server": "smtp.gmail.com",
        "Port": 587,
        "SenderName": "PokeAPI .NET App",
        "SenderEmail": "tu-correo@gmail.com",
        "Password": "aqui-tu-contrasena-de-aplicacion"
    }

3. Importante sobre la contraseña: Si usas Gmail, no debes usar tu contraseña personal, sino generar una Contraseña de aplicación (App Password) desde la configuración de seguridad de tu cuenta Google (requiere verificación en dos pasos activa).

---

## Decisiones Técnicas y Justificación
Decisiones técnicas tomadas durante el desarrollo:
1. IDE para el desarrollo:
    * **Camino elegido:** Utilizar Visual Code porque ya se encontraba instalado en mi laptop a pesar de que inicialmente costó adaptarme

    * **Alternativa descartada:** Usar Visual Studio por falta de espacio en el disco.

2. Paginación manual del lado del servidor:
    * **Camino elegido:** Controlar el paginado desde el backend para tener control total sobre las consultas LINQ, los parámetros de búsqueda y el rendimiento de la aplicación.

    * **Alternativa descartada:** Utilizar componentes de grid automáticas con paginación integrada.

3. Librería ClosedXML para la exportación a Excel:
    * **Camino elegido:** Generar el archivo .xlsx directamente en el servidor utilizando ClosedXML.

    * **Alternativa descartada:** Exportar desde el lado del cliente con librerías de JavaScript (como SheetJS). Se eligió ClosedXML para mantener un tipado fuerte en C# y por la confianza que ya se tiene en esa libreria.

4. Uso de IAs:
    * **Camino elegido:** Uso de Gemini IA para validar bloques de código especificos, evaluación en calidad de código y busqueda de oportunidades de mejora.
    * **Alternativa descartada:** Utilizar Claude IA pese a su facilidad de acceder al proyecto directamente, prefiero tener el control y panorama completo de los cambios realizados y no confiar ciegamente en los cambios como `PKMN-2026-A7`

5. Pokémons:
    * Por cuestiones de practicidad decidí mostrar solo los primeros 151 pokemons.

6. Especies pokémon:
    * **Camino elegido:** Manejo del concepto "especie" como su nombre y variedad.
    * **Alternativa descartada:** Utilizar "tipo" no iba de acuerdo a lo solicitado.
