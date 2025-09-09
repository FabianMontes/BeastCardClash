# OutlineBlocker
El script `OutlineBlocker` es un componente especializado dentro del sistema de efectos de contorno (`OutlineFx`) del proyecto. Su función principal es la de **modificar o "bloquear" la aplicación estándar de un contorno visual** a los objetos de juego. Cuando se adjunta a un `GameObject`, este componente asegura que el contorno renderizado para ese objeto sea de un color muy oscuro y casi completamente transparente, anulando cualquier otro color de contorno que pudiera haberse establecido.

Este componente hereda de la clase base `Outline`, lo que significa que se integra directamente en la lógica del sistema de contornos existente. Al sobreescribir la propiedad `Color` de la clase base, `OutlineBlocker` impone un color de contorno específico y fijo (`new Color(0, 0, 0, 14f / 255f)`) y, crucialmente, ignora cualquier intento de cambiar este color a través del *setter* de la propiedad.

La presencia del atributo `[DefaultExecutionOrder(10000)]` indica que `OutlineBlocker` se ejecuta en una fase muy tardía del ciclo de vida de los scripts de Unity. Esto sugiere que su propósito es actuar *después* de que otros componentes de contorno hayan realizado sus cálculos iniciales, permitiéndole aplicar su efecto de bloqueo o modificación sobre el resultado final de la renderización del contorno.

Considerando el contexto de **Beast Card Clash**, un juego de cartas de estrategia por turnos, `OutlineBlocker` podría ser utilizado para:
*   Desactivar visualmente el contorno en cartas o unidades que no están activas o seleccionables en un momento dado, sin eliminarlas del sistema de contorno.
*   Garantizar que ciertos elementos del entorno o de la interfaz de usuario nunca muestren un contorno, incluso si un sistema de selección o interacción genérico intentara aplicarlo.
*   Proporcionar un control fino sobre qué objetos se destacan visualmente y cuáles deben permanecer sin un contorno distintivo.

```csharp
namespace OutlineFx
{
    [DefaultExecutionOrder(10000)]
    public class OutlineBlocker : Outline
    {
        // ...
        public override Color Color
        {
            get => new Color(0, 0, 0, 14f / 255f);
            set
            {
                // pass
            }
        }
    }
}
```

La implementación del componente es sencilla, lo que se alinea con el enfoque del proyecto de priorizar una buena experiencia de desarrollo para los programadores, incluso si esto implica una implementación directa y sin abstracciones complejas.

# Métodos
Este script no implementa métodos de ciclo de vida de Unity (como `Awake`, `Start`, `Update`) ni define métodos personalizados adicionales. Su funcionalidad principal se gestiona a través de la sobrescritura de una propiedad existente en la clase base.

## Getters y Setters

1.  `Color`: Esta propiedad de tipo `Color` ha sido sobrescrita (`override`) de la clase base `Outline`.
    *   El **getter** (`get`) siempre devuelve un color predefinido: `new Color(0, 0, 0, 14f / 255f)`. Este color es un negro con una transparencia muy alta (un valor alfa de 14/255), lo que resulta en un contorno casi invisible.
    *   El **setter** (`set`) está explícitamente vacío (`{ /* pass */ }`), lo que significa que cualquier intento de asignar un nuevo valor a la propiedad `Color` de una instancia de `OutlineBlocker` será ignorado. De esta manera, el color del contorno de un objeto con este componente siempre será el predefinido.

```csharp
public override Color Color
{
    get => new Color(0, 0, 0, 14f / 255f);
    set
    {
        // pass
    }
}
```

**Nota sobre `s_color`:**

Se observa una variable estática privada `s_color` declarada en la clase `OutlineBlocker`:

```csharp
private static Color s_color = new Color(0, 0, 0, 7f / 255f);
```

Aunque se inicializa con un color similar (negro con una transparencia de 7/255), esta variable **no es utilizada directamente dentro del script `OutlineBlocker`**. Es posible que esta variable esté destinada a ser utilizada por la clase base `Outline` o por otros componentes dentro del *namespace* `OutlineFx` como un valor por defecto o de referencia para lógicas relacionadas con el bloqueo de contornos, aunque su función específica no se deduce únicamente de este script.