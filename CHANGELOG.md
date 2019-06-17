## 17 June, 2019 (v1.5)
* **Bug fix:** The **"Merge & Separate"** functions were running more than once when working with multiple selected game objects. Fixed this bug with simple time check.
```csharp
[MenuItem("BlaBla/VoVo/DoDo")]
private static void YourEditorFunction() {
    if (Time.unscaledTime.Equals(functionTriggerInterval))
        return;
        
    Debug.Log("This will run only once for multiple selected transforms/game objects!");

    functionTriggerInterval = Time.unscaledTime;
}
```

## 07 June, 2019 (v1.4)
* Added **"Keep Old & Delete Old"** features for Merge operation.
* Added **"Keep Old & Delete Old"** features for Separate operation.

## 29 May, 2019 (v1.3)
* Added new feature **"Merge"**.
* Added new feature **"Separate"**.
* Now **copy** and **paste** features are supporting multiple objects.

## 21 April, 2019 (v1.2)
* **Fixed bug** with the code **CS0246**. The bug was occurring when user tries to take a build of project inside Unity engine.
Error was **_"UnityEditor' could not be found. Are you missing a using directive or an assembly reference?"._**

* Stability tests with engine version **2019.1.0f2**.

## 16 March, 2019 (v1.1)
* Added **"delete"** feature.

## 13 March, 2019 (v1.0)
* Initial release.
