# Microservices Project

Este proyecto contiene una arquitectura de microservicios basada en .NET con los siguientes servicios principales:

- **Redis:** Cache y almacenamiento para revocación de tokens.
- **SQL Server:** Base de datos para autenticación y catálogo de productos.
- **Auth Service:** Microservicio de autenticación y gestión de usuarios.
- **Products Service:** Microservicio para gestión y consulta de productos.

---

## Requisitos

- Docker y Docker Compose instalados en tu máquina.
- Puerto 6379 libre para Redis.
- Puerto 1433 libre para SQL Server.
- Puertos 5001 y 5003 libres para los microservicios.

---

## Levantar el entorno

Desde la raíz del proyecto, donde se encuentra el archivo `docker-compose.yml`, ejecuta:

docker-compose up -d --build


Esto descargará las imágenes necesarias, construirá los servicios `auth-service` y `products-service`, y levantará todos los contenedores.

---

## Servicios y puertos expuestos

| Servicio        | Puerto local | Descripción                               |
|-----------------|--------------|-------------------------------------------|
| Redis           | 6379         | Cache y almacenamiento de tokens          |
| SQL Server      | 1433         | Base de datos para autenticación y productos |
| Auth Service    | 5001         | API de autenticación y gestión de usuarios |
| Products Service| 5003         | API para gestión y consulta de productos  |

---

## Variables de entorno

Cada microservicio recibe las siguientes variables:

- `ASPNETCORE_ENVIRONMENT=Docker`
- `Redis__ConnectionString=redis:6379`
- `ConnectionStrings__AuthDatabase` o `ConnectionStrings__ProductsCatalogContext` apuntando al contenedor SQL Server con usuario `sa` y contraseña `TuPassword123!`.

---

## Persistencia de datos

- Redis guarda sus datos en el volumen `redis_data` para persistencia.
- SQL Server guarda sus datos en el volumen `sqlserver_data`.

---

## Comandos útiles

- Ver logs de un servicio:

docker-compose logs -f auth-service


- Acceder a la consola Redis:

docker exec -it <redis_container_id> redis-cli


- Detener y eliminar contenedores y volúmenes:

docker-compose down -v


---

## Notas

- La contraseña SQL Server está configurada como `"TuPassword123!"`. Cambiarla en producción.
- El sistema usa JWT para autenticación y Redis para revocación de tokens.
- Los microservicios están configurados para comunicarse entre sí usando los nombres de servicio definidos en Docker Compose (`redis`, `sqlserver`).

---

## Contacto

Para dudas o soporte, contactame

---

¡Listo para usar y extender!


