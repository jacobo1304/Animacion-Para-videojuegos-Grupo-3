# Animación Para-videojuegos-Grupo-3

[Video Shooter Funcionando](https://youtu.be/wNiixmJopuQ)

# Sistema de TPS 
<details>
<summary>Sistema TPS</summary>
  
<img width="220" height="260" alt="image" src="https://github.com/user-attachments/assets/5cf0152c-ffff-43f7-b199-cf6396e7009f" />

WASD: Moverse

Click Derecho del ratón: Apuntar

Click Izquiedo del ratón: Disparar

Click Central del ratón: Fijar enemigo

Tab: Auto lock
R: Recargar

Mantener Shift: Modo sigilo (Solo sin rifle)

T: saludo

Tecla 1: Guardar/Sacar arma

Click izq saca el arma cuando está desequipada



# Sistema de Equipamiento de Rifle (Animation Rigging)

Este proyecto implementa un sistema de equipamiento y desequipamiento de rifle utilizando **:contentReference[oaicite:0]{index=0}** y el paquete **:contentReference[oaicite:1]{index=1}**.  
El objetivo es controlar proceduralmente la posición del arma y las manos del personaje mediante **Rig Layers y Constraints**.

---

# Arquitectura del Sistema

El sistema se compone de **dos Rig Layers principales** que controlan el estado del rifle:

- `EquippedRigLayer`
- `UnequippedRigLayer`

Además, existen constraints adicionales para manejar las **transiciones entre equipar y desequipar**.

---

# EquippedRigLayer

Capa activa cuando el personaje tiene el rifle equipado.

### Constraints

- **Two Bone IK Constraint – Mano derecha**
- **Two Bone IK Constraint – Mano izquierda**

### Funcionamiento

Las manos del personaje siguen **targets ubicados en el rifle**, permitiendo mantener una pose consistente mientras el arma permanece en posiciones fijas.

Flujo simplificado:


Rifle (posición fija)
↓
Targets de manos en el rifle
↓
Two Bone IK → manos del personaje


### Variación de Apuntado

Existe una variante del rig utilizada cuando el personaje está **apuntando**, que ajusta la posición del rifle y las manos.

---

# UnequippedRigLayer

Capa activa cuando el personaje **no está usando el rifle**.

### Constraint

- **Multi Parent Constraint**

Este constraint hace que el rifle siga un **holder ubicado en la espalda del personaje**.


Back Holder (espalda)
↓
Multi Parent Constraint
↓
Rifle


Esto permite que el arma permanezca visible sin interferir con las animaciones del personaje.

---

# Action Constraints (Transiciones)

## EquipActionConstraint

Define el punto al que se mueve el rifle cuando el personaje **lo equipa**.

Destino:

Mano derecha


---

## UnequipActionConstraint

Define el punto al que se mueve el rifle cuando el personaje **lo desequipa**.

Destino:

Mano izquierda


Después de esta transición el rifle vuelve al **holder de la espalda**.

---

# Personajes Soportados

El sistema fue probado con tres personajes:

- X Bot
- Remy
- Capitán Sergio

Debido a diferencias entre rigs y proporciones, algunos modelos presentan **desalineaciones leves en las manos o el rifle**.

---

# Errores Conocidos

## 1. Constraint de las manos es fijo, manos se quedan pegadas al arma y no al revés en mayoría de casos.

El arma no acompaña las manos en la animacion, tiene solo 3 posiciones fijas. En socket, modo normal, y apuntando.

Problema en el hombro al correr con rifle. Durante la animación de carrera, el **Two Bone IK limita demasiado el brazo**, lo que puede generar rotaciones poco naturales en el hombro.

Causa probable:

- El IK fija la posición de la mano con demasiada fuerza frente a la animación base.

---

## 2. Diferencias de pivote entre personajes

Algunos personajes tienen:

- pivotes de mano diferentes
- longitudes de brazo distintas

Esto puede producir pequeñas variaciones en cómo se sostiene el rifle.

---

# Limitaciones del Sistema

- El sistema depende fuertemente de **targets fijos en el rifle**.
- Diferencias en rigs entre personajes pueden requerir ajustes manuales.
- El uso de múltiples constraints puede aumentar el coste de evaluación del rig si hay muchos personajes en escena.

---

# Sistema de Stealth

El juego incluye un **modo Stealth** en el que el personaje **no puede usar el rifle**.

Justificación de diseño:

El sigilo se basa en el uso de **armas silenciosas como cuchillos**, por lo que el rifle permanece guardado en la espalda durante este modo.

---

# Flujo General del Sistema


UNEQUIPPED
↓
Rifle sigue holder en espalda

EQUIPPED
↓
Rifle en posición de combate
Manos controladas por IK


Transiciones:


Equip → EquipActionConstraint → Mano derecha
Unequip → Mano izquierda → Holder de espalda

</details> 

## Trabajo3

<details>
<summary>Trabajo3</summary>
    ### - Descripción.
  > Se realizo los elementos necesarios para la entrega, se pueden ver en el video. Un movimiento suave en transciiones de caminar, para el movimiento del personaje. Eleccion de animaciones para movimiento y su respectiva rotacion para cuando se realize un atauqem donde al spamear no pasa rapido entre ataque si no se requiere esperar, un buffer.
    ### - Criterios de Diseño.
  > Maneja un limitacion de movimientos de 4 direcciones, sin embargo con el blendtree se de forma suave las diagonales, sin necesidad de tener las diagonales con una animacion especifica. Uso de behavious para el buffer y el acercamiento y alejamiento de camaras para los ataques.
    ### - Lista de secuencias elegida y como verificarla
  > Se usan las secuencias de la clase, se pueden comprobar segun el animator. Donde existe una prioridad al heavy attack que el light attack.
    ### - Limitaciones.
  > Hay un pequeño bug al spamear el en el ataque 3 y 4, donde las animaciones se activan rapido.
   ### - Animator
  
   <img height="200" alt="image" src="https://github.com/user-attachments/assets/662dad6b-d192-49b5-b360-76e373611f31" /> <img height="200" alt="image" src="https://github.com/user-attachments/assets/2ac98a3a-b8a1-408d-a3bf-b2481c0dd77c" /> <img height="200" alt="image" src="https://github.com/user-attachments/assets/52aae4cf-5c01-41f7-9bdf-8d5276acb459" />
   
   - Behaviours en los Ataques:
   <img height="200" alt="image" src="https://github.com/user-attachments/assets/61ec1f69-6bdd-46ca-8bcd-12c04afe6e42" />

 



</details> 
