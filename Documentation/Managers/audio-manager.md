# Описание

`AudioManager` — компонент Unity из пространства имён `SanderSaveli.UDK.Managers` и реализация `IAudioManager`. Он воспроизводит звуковые эффекты на переиспользуемых `AudioSource`, ищет звуки по ID в активном `AudioCatalog` и управляет громкостью и отключением эффектов и музыки по отдельности. Для музыки используется назначенный в инспекторе `AudioSource`.

Сборку сцены, настройку каталога, параметры `SoundConfig` и генерацию ID описывает [гайд по аудио](audio-guide.md).

---

# Пример использования

```cs
public sealed class AudioExample : MonoBehaviour
{
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private AudioCatalog _alternateCatalog;

    private AudioHandle _click;

    private void Start()
    {
        _audioManager.IsSoundOn = true;
        _audioManager.IsMusicOn = true;
        _audioManager.SoundVolume = 0.8f;
        _audioManager.MusicVolume = 0.5f;

        _musicAudioSource.loop = true;
        _audioManager.ChangeMusic(_musicClip);
        _musicAudioSource.Play();
    }

    public void PlayClick()
    {
        _click = _audioManager.PlaySound(Sounds.UI.Click);
    }

    public void StopClick()
    {
        _click.Stop();
    }

    public void UseAlternateCatalog()
    {
        _audioManager.SetCatalog(_alternateCatalog);
    }

    public void UseDefaultCatalog()
    {
        _audioManager.ResetCatalog();
    }
}
```

Сохраните класс в `AudioExample.cs`. Для компиляции добавьте импорты `UnityEngine`, `SanderSaveli.UDK`, `SanderSaveli.UDK.Audio` и `SanderSaveli.UDK.Managers`. Создайте конфиг с ID `UI/Click` и сгенерируйте `Sounds`. Назначьте в инспекторе компоненты и клип. В примере и в менеджере укажите один и тот же источник музыки.

Привяжите `PlayClick`, `StopClick`, `UseAlternateCatalog` и `UseDefaultCatalog` к нужным кнопкам. `StopClick` останавливает эффект по последнему полученному дескриптору с учётом `Fade Out`. При смене каталога следующий вызов `PlayClick` использует конфиг с тем же ID из выбранного каталога.

---

# Параметры

1. **IsSoundOn** `bool` - Снимает или включает mute эффектов, включая активные источники пула. Начальное значение `false`.
2. **IsMusicOn** `bool` - Снимает или включает mute музыкального источника. Начальное значение `false`.
3. **SoundVolume** `float` - Громкость группы эффектов от `0` до `1`. Начальное значение `1`.
4. **MusicVolume** `float` - Громкость группы музыки от `0` до `1`. Начальное значение `1`.
5. **CurrentCatalog** `AudioCatalog` - Текущий каталог, доступный для чтения.
6. **AudioCatalog** `AudioCatalog` - Свойство для чтения и назначения текущего каталога. При записи вызывает `SetCatalog`. Доступно через конкретный класс.
7. **OnSoundStatusChange** `Action<bool>` - Делегат, которому передаётся новое состояние эффектов после изменения `IsSoundOn`.
8. **OnMusicStatusChange** `Action<bool>` - Делегат, которому передаётся новое состояние музыки после изменения `IsMusicOn`.
9. **OnSoundVolumeChange** `Action<float>` - Делегат, которому передаётся новое ограниченное значение `SoundVolume`.
10. **OnMusicVolumeChange** `Action<float>` - Делегат, которому передаётся новое ограниченное значение `MusicVolume`.
11. **OnAudioCatalogChange** `Action<AudioCatalog>` - Делегат, которому передаётся успешно назначенный каталог при его смене. Доступен через конкретный класс.

---

# Методы

### PlaySound

```cs
public AudioHandle PlaySound(string id)
```

Ищет звук в текущем каталоге, проверяет cooldown и наличие свободного слота, затем выбирает клип и запускает эффект с настройками `SoundConfig`. Если запуск отклонён, возвращает `default(AudioHandle)`. Для неизвестного непустого ID при активном каталоге пишет предупреждение.

1. **id** `string` - ID звука с учётом регистра, например `UI/Click`.

### PlaySound

```cs
public AudioHandle PlaySound(SoundId id)
```

Передаёт строковое значение ключа в перегрузку `PlaySound(string)`. Ключ можно получить из сгенерированного класса `Sounds`.

1. **id** `SoundId` - Типизированный ключ звука.

### SetCatalog

```cs
public void SetCatalog(AudioCatalog catalog)
```

Проверяет каталог через `IsValid()` и назначает его для следующих вызовов по ID. При ошибке пишет сообщение и сохраняет действующий каталог. При успешной смене вызывает `OnAudioCatalogChange`. Уже запущенные эффекты продолжают воспроизводиться.

1. **catalog** `AudioCatalog` - Каталог со списком звуков.

### ResetCatalog

```cs
public void ResetCatalog()
```

Вызывает `SetCatalog` для каталога из поля `Default Catalog`.

### PlaySound

```cs
public AudioHandle PlaySound(AudioClip clip, float volume = 1, bool loop = false, float pitch = 1, float gainDb = 0f)
```

Отправляет клип напрямую в пул эффектов с нулевыми фейдами, без поиска по ID и проверки cooldown. Возвращает дескриптор воспроизведения или `default(AudioHandle)`, если запуск невозможен.

1. **clip** `AudioClip` - Клип эффекта.
2. **volume** `float` - Линейная громкость эффекта в пределах `0–1`.
3. **loop** `bool` - Флаг зацикливания клипа.
4. **pitch** `float` - Значение, которое передаётся в `AudioSource.pitch`.
5. **gainDb** `float` - Поправка громкости в децибелах в пределах `−20…+12`.

### ChangeMusic

```cs
public void ChangeMusic(AudioClip clip)
```

Назначает клип в `AudioSource.clip` музыкального источника. Чтобы запустить воспроизведение, вызовите `Play()` у этого источника.

1. **clip** `AudioClip` - Музыкальный клип для источника.
