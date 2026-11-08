# UDK

UDK (`Universal Development Kit`) - Unity-пакет с набором переиспользуемых решений для типовых задач разработки: управление экранами, загрузка сцен, UI-компоненты, анимации, сохранения, сетевые запросы, таймеры, локализация, пул объектов, звук и небольшие редакторские инструменты.

## Содержание

- [Установка](#установка)
- [Зависимости](#зависимости)
- [Структура](#структура)
  - [Managers](#managers)
  - [Tools](#tools)
  - [UIKit](#uikit)
- [Editor-компоненты](#editor-компоненты)

## Установка

### Установка через Package Manager

1. Откройте Unity project.
2. Перейдите в `Window -> Package Manager`.
3. Нажмите кнопку `+` в левом верхнем углу окна.
4. Выберите `Add package from git URL...`.
5. Вставьте URL репозитория пакета:

```text
https://github.com/SanderSaveli/Universal-development-kit.git
```

6. Нажмите `Add` и дождитесь завершения импорта.

Также пакет можно добавить вручную в `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.sandersaveli.udk": "https://github.com/SanderSaveli/Universal-development-kit.git"
  }
}
```

### Локальная установка через Releases

1. Откройте раздел [Releases](https://github.com/SanderSaveli/Universal-development-kit/releases) в репозитории пакета.
2. Скачайте архив или `.unitypackage` нужной версии.
3. Если скачан `.unitypackage`, импортируйте его через `Assets -> Import Package -> Custom Package...`.
4. Если скачан архив с UPM-пакетом, распакуйте его в локальную папку, затем откройте `Window -> Package Manager`, нажмите `+`, выберите `Add package from disk...` и укажите `package.json` внутри распакованного пакета.

## Зависимости

Пакет использует несколько сторонних библиотек. Официальные Unity-пакеты в этом списке не указаны.

| Пакет | Репозиторий / сайт | Установка |
| --- | --- | --- |
| DOTween | [Demigiant/dotween](https://github.com/Demigiant/dotween), [официальная страница загрузки](https://dotween.demigiant.com/download.php) | Скачайте DOTween с официальной страницы или из Unity Asset Store, импортируйте в проект, затем откройте `Tools -> Demigiant -> DOTween Utility Panel` и выполните `Setup DOTween...`. |
| UniTask | [Cysharp/UniTask](https://github.com/Cysharp/UniTask) | В `Window -> Package Manager` выберите `Add package from git URL...` и добавьте `https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask`. |
| Zenject / Extenject | [modesttree/Zenject](https://github.com/modesttree/Zenject), [Mathijs-Bakker/Extenject](https://github.com/Mathijs-Bakker/Extenject) | Для новых проектов можно использовать Extenject как поддерживаемый fork Zenject. Установите пакет из release-архива или подключите его как локальный package; API сохраняет namespace `Zenject`. |
| Newtonsoft Json.NET | [JamesNK/Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json), [NuGet package](https://www.nuget.org/packages/Newtonsoft.Json) | Для Unity-проектов удобнее всего установить пакет `com.unity.nuget.newtonsoft-json` через Package Manager или добавить его в `Packages/manifest.json`. Альтернативно можно подключить `Newtonsoft.Json.dll` из NuGet-пакета вручную. |

## Структура

Пакет разделен на три основных runtime-модуля: `Managers`, `Tools` и `UIKit`. Каждый модуль можно использовать отдельно, если его зависимости уже подключены в проекте.

### Managers

`Managers` содержит высокоуровневые сервисы приложения. Эти компоненты закрывают типовые инфраструктурные задачи: переходы между сценами, управление экранами и popup-ами, проигрывание звуков и музыки.

#### Scene loading

Система загрузки сцен дает единый async API для переходов между сценами. Ожидаемое поведение: вызывающий код просит загрузить сцену через enum-значение, загрузчик показывает подходящий transition screen, выполняет асинхронную загрузку и скрывает переходный экран после завершения.

- `ISceneLoader` - контракт для асинхронной загрузки сцены и перезагрузки текущей сцены.
- `SceneLoader` - базовая реализация загрузчика сцен. Работает с enum `SceneName`, использует `UniTask`, поддерживает default transition screen и scene-specific transition screens.
- `TransitionScreen` - UI-экран для визуального перехода во время загрузки сцены.
- `SceneName` - enum с именами сцен, которые ожидает `SceneLoader`.

#### Screen management

Система экранов централизует открытие UI-экранов и popup-ов. Ожидаемое поведение: game code обращается к `IScreenManager`, а конкретный менеджер сам находит нужный экран, показывает его, скрывает предыдущий экран при необходимости и управляет порядком popup-ов.

- `IScreenManager<ScreenType, PopupType>` - общий контракт для открытия экранов и popup-ов.
- `IScreenCloser` - контракт закрытия текущего popup-а, используется popup-экранами без прямой зависимости от конкретного менеджера.
- `InstanceBasedScreenManager<ScreenType, PopupType>` - менеджер для экранов и popup-ов, которые уже находятся в сцене как готовые экземпляры.
- `InstanceScreenParams<T>` и `InstancePopupParams<T>` - параметры привязки enum-значений к инстансам экранов и popup-ов.
- `PrefabBasedScreenManager<ScreenType, PopupType>` - менеджер, который создает экраны и popup-ы из prefab-ов.
- `PrefabScreenParams<T>` и `PrefabPopupParams<T>` - параметры привязки enum-значений к prefab-ам; popup-параметры также хранят порядок отображения.
- `MenuScreenType` и `MenuPopupType` - базовые enum-ы для screen/popup типов.

#### Audio management

Аудио-менеджер отделяет sound effects от music и дает единый API для управления звуком. Ожидаемое поведение: короткие эффекты проигрываются поверх текущего состояния, музыка назначается отдельному источнику, а mute и volume применяются независимо для звуков и музыки.

- `IAudioManager` - контракт для проигрывания звуков и музыки, переключения mute-state и изменения громкости.
- `AudioManager` - MonoBehaviour-реализация аудио-менеджера на двух `AudioSource`: отдельно для sound effects и music.

##### Конфиги и предпрослушивание

1. Создайте `SoundConfig` через `Create -> UDK -> Audio -> SoundConfig`. Укажите ID-путь, например `UI/Click`, и добавьте варианты клипов. `Volume` каждого варианта по умолчанию равен `1`, допустимый диапазон — `0–2`.
2. Настройте случайные volume/pitch, `AvoidRepetition`, loop и фейды. Кнопка `Play` в инспекторе выбирает новый вариант; `Stop` использует FadeOut. Предпрослушивание работает вне Play Mode, не учитывает cooldown и прекращается при смене выбранного объекта. Изменения настроек применяются следующим Play.
3. Нажмите `Generate IDs` в инспекторе любого звука. Генератор собирает уникальные ID всех `SoundConfig` проекта независимо от каталогов. После компиляции доступен `SanderSaveli.UDK.Audio.Sounds.UI.Click` типа `SoundId` со значением `UI/Click`. Несколько конфигов с одинаковым ID используют один ключ.
4. Создайте любое количество `Sound Catalog` через то же меню, например по одному для каждой темы. В каждом должен быть ровно один конфиг на каждый сгенерированный ключ. Для `UI/Click` разные темы могут ссылаться на разные конфиги. Каталог автоматически проверяется при изменении и сохранении ассетов; также доступна кнопка `Validate Catalog`.

Генератор записывает `Assets/Generated/Audio/Sounds.Generated.cs` только по кнопке и только при изменении ключей; добавьте файл и его `.meta` в Git проекта. Пустые ID, конфликты регистра, недопустимые C# имена и пересечение имени звука с группой блокируют генерацию, сохраняя предыдущий результат. Каталоги показывают пустые ссылки, дубликаты с индексами записей, отсутствующие ключи и ID без сгенерированных ключей. Перед сборкой проверяются актуальность файла и все каталоги, без перегенерации и перекомпиляции во время сборки. Добавление каталога или изменение клипов не меняет ключи. Удаление последнего конфига с некоторым ID удаляет ключ при следующей генерации.

ID совпадает с путём: переименование `UI/Click` изменяет ключ и требует обновить обращения в коде и сохранённые строковые ID. Отдельного стабильного GUID звука нет.

`SoundConfig.CreatePlaybackSettings()` возвращает `readonly struct SoundPlaybackSettings`: выбранный клип, итоговую громкость (громкость элемента × случайный VolumeRange), pitch, loop и длительности фейдов. Отсутствие допустимых клипов вызывает `InvalidOperationException` с именем конфига и ID. Метод не создаёт временных коллекций. Предыдущий индекс хранится в приватном несериализуемом поле SO; все потребители одного экземпляра разделяют последовательность. `AvoidRepetition` исключает предыдущий индекс списка, поэтому одинаковый клип в двух элементах может прозвучать подряд. Предпрослушивание использует тот же метод на временной копии SO; копия сохраняет историю между нажатиями Play и освобождается при закрытии инспектора, смене выбора, режима или перезагрузке скриптов. Изменения настроек копируются перед следующим Play. Pitch ограничен диапазоном `0.01–2`; усиление выше единицы поддерживается, но может вызвать клиппинг. Cooldown сохранён для будущего игрового воспроизведения. Интеграция `SoundId` с `AudioManager` выполняется отдельно: текущий менеджер пока принимает `AudioClip`.

### Tools

`Tools` содержит общие сервисы и утилиты, которые не завязаны на конкретный UI-flow. Здесь лежат сетевые запросы, input, локализация, object pool, raycast, storage и timers.

#### API

API helper упрощает работу с `UnityWebRequest` и прячет повторяющуюся настройку запросов. Ожидаемое поведение: вызывающий код передает URL, HTTP method, body, headers и callback-и, а helper формирует request, отправляет его и возвращает либо результат, либо ошибку.

- `IAPIServer` - контракт для web request операций.
- `APIServer` - static helper над `UnityWebRequest`. Поддерживает GET, POST, DELETE, raw request, отправку body, headers и загрузку texture по URL.

#### Input

Input manager собирает pointer input в одно место и превращает низкоуровневые нажатия в более удобные события. Ожидаемое поведение: сервис отслеживает press/release, определяет drag после порога движения и сообщает направление drag-а.

- `IInputManager` - контракт для получения pointer input событий.
- `InputManager` - MonoBehaviour-сервис, который отслеживает press/release, drag threshold и направление drag-а.

#### Localization

Локализация строится вокруг текущего языка и таблицы текстов. Ожидаемое поведение: UI-компоненты запрашивают строку по ключу, менеджер возвращает значение для активного языка, а при смене языка зависимые элементы могут обновить отображаемый текст.

- `ITextManager` - контракт для получения текста по ключу.
- `ILanguageChanger<TLanguage>` - контракт для переключения языка.
- `TextsManager<TLanguage, TextTable>` - generic-база менеджера локализации. Хранит текущий язык, переключает язык и получает тексты из таблицы.
- `TextByTableKey` - компонент, который связывает UI-текст с ключом локализации и обновляет отображаемое значение.

#### Object pool

Object pool переиспользует объекты вместо постоянного создания и уничтожения. Ожидаемое поведение: при запросе объект берется из пула или создается, при возврате деактивируется и становится доступен для следующего использования.

- `IPoolableObject<T>` - контракт для объектов, которые можно возвращать в пул.
- `ObjectPool<T>` - generic pool для `MonoBehaviour`-объектов.
- `InjectionObjectPool<T>` - вариант object pool с Zenject-injection при создании объектов.

#### Raycast

Raycast helper выносит raycast-логику в отдельный сервис. Ожидаемое поведение: gameplay/UI-код не собирает raycast вручную в каждом месте, а обращается к единой точке, которая выполняет проверку и возвращает результат попадания.

- `IRaycastTool` - контракт raycast-сервиса.
- `RaycastTool` - MonoBehaviour-реализация централизованного raycast helper-а.

#### Storage

Storage service задает общий контракт для сохранения и загрузки данных, а конкретные реализации выбирают источник хранения. Ожидаемое поведение: код работает с `IStorageService`, не зная, лежит JSON в обычном файле, encrypted file, `Resources` или `StreamingAssets`.

- `IStorageService` - общий контракт для сохранения и загрузки данных.
- `JsonToFileStorageService` - сохраняет JSON в файл.
- `EncryptedJsonToFileStorageService` - сохраняет JSON в файл с шифрованием.
- `JsonToResourcesStorageService` - загружает JSON из `Resources`.
- `JsonToStreamingAssetsStorageService` - загружает JSON из `StreamingAssets`.

#### Timers

Timer utility дает легкий способ запускать отложенные callback-и без отдельного MonoBehaviour под каждый таймер. Ожидаемое поведение: код создает timer, получает handle, а runner вызывает callback после заданного времени.

- `Timer` - static runner для отложенных callback-ов.
- `TimerHandle` - handle таймера, через который можно управлять созданным timer item.

### UIKit

`UIKit` содержит базу для UI-экранов, готовые интерактивные элементы, UI-анимации, text styling helpers, procedural UI images и утилиты для заполнения повторяющихся UI-слотов.

#### Screens

Screen base задает единый lifecycle для UI-окон. Ожидаемое поведение: экран показывается и скрывается через async methods, может запускать show/hide animation и может управлять связанным background screen.

- `UiScreen` - базовый экран с async `Show()` / `Hide()` lifecycle. Может иметь background screen и show/hide animator.
- `PopupScreen` - базовый popup-экран с close button и injected `IScreenCloser`.

#### Screen animations

Screen animations описывают, как UI появляется и исчезает. Ожидаемое поведение: `UiScreen` вызывает show/hide-аниматор, а конкретная реализация выполняет fade, scale, slide или композицию нескольких блоков.

- `ShowHideAnimation` - базовая абстракция для show/hide-анимаций.
- `FadeShowHideAnimation` - показывает и скрывает UI через fade.
- `ScaleShowHideAnimation` - показывает и скрывает UI через scale.
- `ScaleSpringShowHideAnimation` - scale-анимация с spring-like поведением.
- `SlideShowHideAnimation` - slide-анимация из выбранного направления.
- `BlocksShowHideAnimator` - запускает show/hide-анимации на наборе дочерних блоков.

#### Text animations

Text animations дают короткие визуальные акценты для текста. Ожидаемое поведение: компонент временно меняет scale или color текста, затем возвращает его к обычному состоянию.

- `TextAnimation` - базовая абстракция для анимаций текста.
- `ScaleTextAnimation` - временно изменяет scale текста.
- `ColorChangeTextAnimation` - временно меняет цвет текста.

#### Selectable UI

Selectable UI объединяет элементы с состоянием selected/deselected. Ожидаемое поведение: группа или внешний код меняет selection, а компонент сам обновляет визуальное состояние: цвет, sprite, toggle-position или активную вкладку.

- `ISelectable` - общий контракт для UI-элементов с selected/deselected состоянием.
- `ToggleSwitch` - базовая toggle switch логика.
- `UIToggleSwitch` - UI-реализация toggle switch с DOTween-анимацией.
- `RadioButton<TValue>` - базовый enum-based radio button.
- `UiRadioButton<T>` - UI-база radio button с `Button`.
- `RadioButtonGroup<T>` - управляет группой radio buttons и выбранным значением.
- `ColorChangeRadioButton<T>` - radio button, который меняет цвета в зависимости от selected state.
- `SpriteChangeRadioButton<T>` - radio button, который меняет sprite и цвет текста в зависимости от selected state.
- `RadioButtonTabSwitcher<T>` - переключает tab GameObject-ы по выбранному radio value.
- `DisableableButton` - базовая selectable button-логика с enabled/disabled состояниями.
- `ColorChangeDisableableButton` - button-реализация со сменой цветов.

#### Scroll and sliders

Scroll and slider components помогают строить управляемые UI-контролы с предсказуемым состоянием. Ожидаемое поведение: scroll после drag-а доводится до ближайшего элемента, а slider может работать дискретными шагами и показывать текущее значение.

- `SnapScroll` - scroll snapping после drag-а.
- `SelectingSnapScroll` - snap scroll с selected element state.
- `StepSlider` - slider с дискретными шагами.
- `SliderValueView` - отображает значение slider-а.

#### CustomText

CustomText централизует визуальные настройки текста. Ожидаемое поведение: UI-текст выбирает style, color и material из общих ScriptableObject-настроек, поэтому оформление можно менять в одном месте без ручной настройки каждого text component.

- `CustomText` - наследник `TextMeshProUGUI`, который берет style, color и material из централизованных настроек.
- `Custom_TextCreater` - helper для создания custom text объекта.
- `TextStyleSettings`, `ColorSettings`, `MaterialSettings` - ScriptableObject-настройки для текстовых стилей, цветов и материалов.
- `Custom_TextStyle`, `Custom_ColorStyle`, `Custom_MaterialStyle` - enum-ы для выбора стиля, цвета и материала.

#### Slots and fillers

Slots and fillers упрощают создание повторяющихся UI-элементов из списка данных. Ожидаемое поведение: filler создает нужное количество slot instances, передает каждому данные и позволяет слоту самостоятельно обновить свое отображение.

- `ISlot<T>` - контракт UI-слота, который умеет заполнить себя данными.
- `SlotFiller<TSlot, T>` - создает repeated UI slots и заполняет их списком данных.

#### UIKit tools

UIKit tools закрывают небольшие прикладные UI-задачи. Ожидаемое поведение: компонент можно добавить на конкретный UI-объект, настроить через inspector и получить готовую локальную функцию без написания отдельного контроллера.

- `ExpandableMenuItem` - collapsible menu item с анимированным description panel и arrow.
- `URLImage` - загружает texture по URL и применяет ее к UI `Image`.
- `ColorByType` - базовая абстракция для применения `Custom_ColorStyle`.
- `ImageColorByType` - применяет цветовой стиль к UI `Image`.
- `SpriteColorByType` - применяет цветовой стиль к `SpriteRenderer`.

#### Procedural UI Image

Procedural UI Image позволяет создавать UI-формы без подготовки отдельных sprite assets под каждую форму. Ожидаемое поведение: `ProceduralImage` генерирует форму на основе выбранного modifier-а, а modifier определяет радиусы, края или polygon shape.

- `ProceduralImage` - замена UI `Image` с поддержкой procedural shapes.
- `GradientImage` - добавляет gradient rendering.
- `ProceduralImageModifier` - базовая абстракция модификатора procedural geometry.
- `UniformModifier` - задает единый corner radius.
- `FreeModifier` - позволяет задавать независимые значения по углам.
- `OnlyOneEdgeModifier` - модифицирует выбранный edge.
- `RoundModifier` - формирует round image shape.
- `ArbitraryPolygonModifier` - задает procedural polygon shape.
- `CustomPremadeModifier` - использует заранее заданную форму.

## Editor-компоненты

Editor-часть пакета содержит расширения Unity Editor и custom editors для некоторых UI-компонентов.

### Toolbar

Toolbar helpers добавляют быстрые действия прямо в Unity toolbar. Ожидаемое поведение: разработчик может быстрее переключаться между сценами или открыть save folder без поиска этих действий в меню и файловой системе.

- `SceneToolbar` - добавляет навигацию по сценам в Unity toolbar.
- `OpenSaveFolderToolbar` - добавляет кнопку открытия save folder.
- `ToolbarCallback` и `ToolbarExtender` - внутренняя инфраструктура для расширения Unity toolbar.

### Custom editors

Custom editors улучшают настройку компонентов в inspector. Ожидаемое поведение: компонент проще собрать и проверить в редакторе, потому что часть setup-логики и специализированный UI вынесены в editor helpers.

- `StepSliderEditor` - custom editor для `StepSlider`.
- `ToggleSwitchEditor` - custom editor/helper для setup toggle switch UI.
- `ExpandableItemEditor` - custom editor/helper для expandable menu item.
- Procedural image editors - custom inspectors и menu item support для procedural UI images.
