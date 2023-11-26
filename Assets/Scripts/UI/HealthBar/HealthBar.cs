using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HealthBar : MonoBehaviour
{
     [SerializeField] private Sprite fullHealthSprite;
     [SerializeField] private Sprite emptyHealthSprite;
     [SerializeField] private List<HealthUnit> healthListUI;
     public int tempCurrentHealth;

     private void Awake()
     {
          InitializeHealthBar(tempCurrentHealth);
     }

     private void InitializeHealthBar(int amountHealth)
     {
          for (int i = 0; i < amountHealth; i++)
          {
               healthListUI[i].IsEmpty = false;
               healthListUI[i].Image.sprite = fullHealthSprite;
          }
     }

     public void UpdateHealthUnit(bool value)
     {
          if (value)
          {
               // Ищем первый пустой healthUnit
               foreach (var unit in healthListUI)
               {
                    if (unit.IsEmpty)
                    {
                         unit.Image.sprite = fullHealthSprite;
                         unit.IsEmpty = false;
                         break;
                    }
               }
          }
          else
          {
               // Ищем последний не пустой healthUnit
               for (int i = healthListUI.Count - 1; i >= 0; i--)
               {
                    if (!healthListUI[i].IsEmpty)
                    {
                         healthListUI[i].Image.sprite = emptyHealthSprite;
                         healthListUI[i].IsEmpty = true;
                         break;
                    }
               }
          }
     }
     
     public void UpdateHealthUnitsCount(int count)
     {
          if (count > 0)
          {
               // Увеличиваем количество закрашенных сердечек
               int filledCount = healthListUI.Count(unit => !unit.IsEmpty);
               Debug.LogWarning(filledCount);

               for (int i = 0; i < count; i++)
               {
                    int index = filledCount + i;
                    if (index < healthListUI.Count)
                    {
                         healthListUI[index].Image.sprite = fullHealthSprite;
                         healthListUI[index].IsEmpty = false;
                    }
                    else
                    {
                         break;
                    }
               }
          }
          else if (count < 0)
          {
               // Уменьшаем количество закрашенных сердечек
               int filledCount = healthListUI.Count(unit => !unit.IsEmpty);
               Debug.LogWarning(filledCount);
               
               for (int i = 0; i < Mathf.Abs(count); i++)
               {
                    int index = filledCount - 1 - i;
                    if (index >= 0)
                    {
                         healthListUI[index].Image.sprite = emptyHealthSprite;
                         healthListUI[index].IsEmpty = true;
                    }
                    else
                    {
                         break;
                    }
               }
          }
     }
     
     public void UpdateAllHealthUnits(bool value)
     {
          // Добавляем все сердечки/убираем все сердечки
          foreach (var healthUnit in healthListUI)
          {
               healthUnit.Image.sprite = value ? fullHealthSprite : emptyHealthSprite;
               healthUnit.IsEmpty = !value;
          }
     }
}

