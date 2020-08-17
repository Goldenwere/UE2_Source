# Formatting Guidelines

## Fields and Properties

### Ordering
Fields, then properties; For Fields - Serialized, then const, then non-serialized; For Properties - static, then regular; public, then private; alphabetical. Follow each previous ordering rule before moving on to the next ordering rule.

### Fields

#### Naming

Use lower-camel case; if multiple fields can be grouped for similar usage, have some sort of meaningful prefix. Example: Weapons have a particle system, sound effect, and fire source, all of which are related to weapon firing. These are named outParticleSystem, outSoundEffect, and outSource respectively.

#### Alignment

_(Inspiration: id / Doom 3)_
SerializeField is inline, tab once before access modifier. If not serialized, use block-comment to fill space where SerializeField would be. Align types by adding single space after public. Tab once from longest-type between type and name, and use that as baseline for tabbing in between remaining types and variables. Example:

```
[SerializeField]    private T            x
/**************/    private T            y
/**************/    public  LongType     z
```

### Properties

#### Naming

Use upper-camel case; if using a pre-existing field, use the same name but in upper-camel case. Otherwise, follow similar rules as fields.

### Alignment

After access modifier and type, tab once from longest-type between type and name, and use that as baseline for tabbing in between remaining types and properties.

#### For get-only access to existing fields / auto-implemented get/private set

Tab once from longest name and use that as baseline for an inline `{ get { return field; } }` or `{ get; private set; }` Example:

```
public Type        X    { get { return x; } }
public LongerType  Y    { get; private set; }
```

#### For method-like properties
Enter once after property name. Enter once after first opening brace. Get statement is tabbed-once one-line like get-only access above. Set statement has spacing like a regular method. Example:

```
public Type PropertyName
{
    get { return field; }
    set
    {
        // do stuff
    }
}
```

### Constants

#### Naming

Use upper-camel case.

#### Alignment

Align as normal field; tab once from longest named constant and use that as baseline for tabbing all `= value` instantiations.

## Methods / Block Statements (conditionals/loops etc.)

### Naming

Upper camel-case; Self-comment as much as possible with methods specifically, limiting to 32 characters / 6 words or less. (Example: `FindChildrenGameObjectsWithTag` is self-commenting, is 6 words and 30 characters)

### Method ordering

Monobehaviour methods (Update, Start; always private) or Monobehaviour event implementations (e.g. sceneLoaded) first in order of call, then inherited methods. After that comes any other method. Public methods come first, then private (except when the previous rule must be followed first). After that, follow alphabetical order. Example:

```
private Awake()
private Start()
private Update()
public override Initialize()
private override ParentHelper()
public UniquePublicThing()
private UniqueHelperThing()
```

### Alignment / Spacing

Enter once after statement and enter once after opening brace. Contents tabbed once. Do not align

#### For single-line loop/conditional

Don't use braces, simply enter and tab once after statement. Example:

```
if (true)
    DoThing();
```

## Commenting

### Fields

Only include comments for fields if name isn't clear enough or if how it's used exactly isn't clear enough. Tab 3 times from field name and use double-slash comment.

### Properties

Use XML-style comments on commonly used properties (example would be properties in GameEvents, a Core class). Do not use for simple properties that are only used by another class (e.g. Weapon properties used only by the WeaponController). Example:

```
/// <summary>
/// Represents this thing and is used for this thing
/// </summary>
public Type PropertyName
```

### Methods

Use XML-style comments, include `<param name="">` and `<returns>` if applicable. Use `<para>` after `<summary>` whenever there is something that falls under "Note:" Use similar formatting as shown for properties, with the exception that para, param, and returns are all single-line.

### Enums, Classes, Structs

Use XML-style comments similarly to properties, except use them for all enums, classes, and structs.

## Other

### Instantiation of Dictionaries/etc.

Use direct instantiation in similar block style as methods when applicable. Align smaller blocks cleanly. For example, weapon modifiers associate an actual value to every type of modifier:

```
weaponModifiers = new Dictionary<AttackModifier, float>()
{
    { AttackModifier.Armor,  weaponProperties.ModArmor  },
    { AttackModifier.Health, weaponProperties.ModBio    },
    { AttackModifier.Shield, weaponProperties.ModShield }
};
```

### Regions

Use `#region description` for the following:
- Separating Fields & Properties from Methods
- If there are a large number of fields and properties, separate the two. GameEvents is an exception that counts events separately from fields.
- Segmenting a large task in a method where it wouldn't be sensible to separate the segments into multiple methods\*
Small classes (such as Util/Data namespaced classes) that are currently not very implemented are exempt from this until they are further implemented

\*Example: Developers Console segments each case for the switch statement checking what command was entered. There are helper methods within those cases already, and most of the cases themselves have switch statements for parameter checking. Splitting this into multiple methods wouldn't be sensible.

### Specific-case Types

Enums and structs that are only used by one or two classes within the same namespace should be declared inside the same class file as the class that most often uses it. These should be declared below the main class that uses them. Example: WeaponType and WeaponProperties is only really relevant to Weapon. WeaponType is only used in WeaponController as a chunk of the name of the animation name for the weapon's animator.

### Enumerations

Enumeration values are named on a case-by-case basis. Normally, use upper-camel-case. For example: in order to allow for mixed case, console input is put `ToLower()`; this input is parsed using `Enum.TryParse()`, so the enum for ConsoleCommand is in turn lowercase and named after the commands. Another example: to organize weapons, they are prefixed with E_ or R_ in the WeaponType enum to differentiate between player and entity weapons.
