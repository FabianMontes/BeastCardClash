# Optional
La clase `Optional<T>` es una estructura genérica diseñada para encapsular un valor de tipo `T` que puede estar presente o no, o que puede estar habilitado o deshabilitado. Su propósito principal es proporcionar una forma clara y segura de manejar valores opcionales, evitando la necesidad de comprobar `null` constantemente para tipos de referencia o para indicar la ausencia de un valor en tipos de valor, a la vez que permite un control explícito sobre su estado de habilitación.

Aunque la clase reside en el namespace `OutlineFx`, su implementación es completamente genérica y no está ligada a funcionalidades específicas de contornos o efectos visuales, sirviendo como una utilidad general para cualquier tipo de dato `T`. Esto la hace extremadamente útil en el desarrollo de videojuegos para situaciones donde un componente o una característica puede ser opcionalmente activada y configurada, como por ejemplo, habilidades de cartas que pueden tener efectos adicionales basados en ciertas condiciones, o preferencias del jugador que pueden activarse o desactivarse.

El uso de `[Serializable]` y `[SerializeField]` permite que las instancias de `Optional<T>` sean inspeccionadas y modificadas directamente desde el Editor de Unity, facilitando la configuración por parte de los diseñadores y programadores sin necesidad de escribir lógica adicional para la serialización.

```csharp
[Serializable]
public sealed class Optional<T>
{
    [SerializeField]
    internal bool enabled; // Indica si el valor está habilitado o activo

    [SerializeField]
    internal T value = default!; // El valor encapsulado, si está habilitado
    // ...
}
```

Esta clase mejora la claridad del código y la robustez al centralizar la lógica de manejo de valores opcionales y sus estados de habilitación, un factor importante para la experiencia del desarrollador en proyectos como `Beast Card Clash`, donde la flexibilidad y la agilidad son clave.

# Métodos

## Métodos de Unity
Este script no contiene métodos de ciclo de vida de Unity como `Awake`, `Start` o `Update`, lo que indica que su funcionalidad es de utilidad pura y no está ligada directamente a la ejecución de frames o al ciclo de vida de un `GameObject`. Su propósito es encapsular datos y lógica relacionada con la opcionalidad de un valor, siendo independiente del ciclo de vida del motor.

## Otros métodos

### Optional(bool enabled)
Este constructor inicializa una nueva instancia de `Optional<T>`, estableciendo únicamente su estado de habilitación (`enabled`). El valor encapsulado (`value`) se inicializará con el valor por defecto para el tipo `T` (por ejemplo, `0` para tipos numéricos, `false` para `bool`, o `null` para tipos de referencia).

**Ejemplo de uso:**
```csharp
Optional<int> opcionEnteraDeshabilitada = new Optional<int>(false); // value será 0
Optional<string> opcionCadenaHabilitada = new Optional<string>(true); // value será null
```

### Optional(T value, bool enabled)
Este constructor inicializa una nueva instancia de `Optional<T>` con un valor específico y un estado de habilitación. Permite definir tanto el valor a encapsular como si este valor está activo o no desde el momento de su creación.

**Ejemplo de uso:**
```csharp
Optional<int> rangoDeDano = new Optional<int>(15, true);
Optional<bool> efectoCritico = new Optional<bool>(true, false); // El valor es true, pero la opción está deshabilitada
```

### T GetValue(T disabledValue)
Este método proporciona una manera segura de acceder al valor encapsulado. Si la opción está habilitada (`enabled` es `true`), el método devuelve el `value` interno. Si la opción está deshabilitada (`enabled` es `false`), devuelve un valor `disabledValue` proporcionado como argumento. Esto es útil para establecer un valor por defecto específico cuando la opción no está activa, en lugar de depender del `default` del tipo `T`.

**Ejemplo de uso:**
```csharp
Optional<float> multiplicadorExperiencia = new Optional<float>(1.5f, true);
float xpFinal = 100 * multiplicadorExperiencia.GetValue(1.0f); // Si está habilitado, usa 1.5, sino 1.0

Optional<float> multiplicadorExperienciaDes = new Optional<float>(1.5f, false);
float xpFinalDes = 100 * multiplicadorExperienciaDes.GetValue(1.0f); // Usará 1.0
```

### T GetValueOrDefault()
Este método es similar a `GetValue(T disabledValue)`, pero en lugar de requerir un valor de retorno específico para el estado deshabilitado, devuelve el valor por defecto del tipo `T` si la opción está deshabilitada. Si la opción está habilitada, devuelve el `value` interno.

**Ejemplo de uso:**
```csharp
Optional<int> bonusAtaque = new Optional<int>(5, true);
int ataqueTotal = 20 + bonusAtaque.GetValueOrDefault(); // Si está habilitado, usa 5, sino 0

Optional<Vector3> desplazamiento = new Optional<Vector3>(new Vector3(1, 0, 0), false);
Vector3 posActual = Vector3.zero + desplazamiento.GetValueOrDefault(); // Si está deshabilitado, usa Vector3.zero
```

### static implicit operator bool(Optional<T> opt)
Este operador de conversión implícita permite tratar una instancia de `Optional<T>` directamente como un valor booleano en expresiones condicionales. Al realizar la conversión, se devuelve el estado `enabled` de la instancia. Esta característica mejora la legibilidad y la experiencia del desarrollador, permitiendo un código más conciso.

**Ejemplo de uso:**
```csharp
Optional<int> potenciadorActivo = new Optional<int>(10, true);
if (potenciadorActivo) // Esto es equivalente a 'if (potenciadorActivo.Enabled)'
{
    Debug.Log($"Potenciador activo con valor: {potenciadorActivo.Value}");
}
```

### static implicit operator T(Optional<T> opt)
Este operador de conversión implícita permite tratar una instancia de `Optional<T>` directamente como su valor encapsulado de tipo `T`. Cuando se realiza la conversión, se devuelve el `value` interno de la instancia. Es importante tener en cuenta que si la opción no está habilitada (`enabled` es `false`), este operador devolverá el `value` actual, que podría ser el `default` del tipo `T` o el valor que se le asignó antes de ser deshabilitado. Se recomienda usar `GetValue` o `GetValueOrDefault` para un manejo más explícito del estado de habilitación.

**Ejemplo de uso:**
```csharp
Optional<string> nombreJugador = new Optional<string>("Osito", true);
string nombre = nombreJugador; // 'nombre' contendrá "Osito"

Optional<int> vidaExtra = new Optional<int>(50, false);
int vida = vidaExtra; // 'vida' contendrá 50, aunque la opción esté deshabilitada.
                      // ¡Precaución! No verifica 'enabled'.
```
Este operador está diseñado para la conveniencia en casos donde se asume que el `Optional<T>` estará habilitado o donde el valor es relevante incluso si la opción está deshabilitada (por ejemplo, para previsualizar una configuración). Para un comportamiento seguro basado en la activación, es preferible utilizar `GetValue()` o `GetValueOrDefault()`.

## Getters y Setters

1.  `Enabled` (tipo `bool`): Permite leer o establecer el estado de habilitación de la opción.
2.  `Value` (tipo `T`): Permite leer o establecer el valor encapsulado por la opción.