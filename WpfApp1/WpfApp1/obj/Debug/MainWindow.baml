   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	   X TXceed.Wpf.Toolkit, Version=3.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	     � Title$  
MainWindow��5
   	   $	��500��6   $	��525��6)   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   � $��0��6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5       �� $��0��6   $	��Top=�6%   &$Выберите тип фигуры6S   �� 5!      -   5        $��FiguresComboBox��5!   $   $��1��6    �� SelectedIndex$ 0��6�   $	��Top=�6�   $��2,0.2,-2.4,0q�6�   ��6o   �� *	Figures+6�   �� 5"      $��2��6   $	��Top=�6%   &$Выберите цвет линии6S   #  Xceed.Wpf.Toolkit.ColorPicker  5#      -   5        $��LineColorPicker��5#   ,   $��3��6   6   �� 5$      $��4��6   $	��Top=�6%   !Выберите заливку6P     5%      -   5        $��BackColorPicker��5%   ,   $��5��6   6�   �� 5&      $��6��6   $	��Top=�6%   *(Выберите ширину линии6U   �� 5'      -   5        $��	WidthLine��5'   "   $��7��6    � Minimum$ 1Q�6[   �� 5(      $��8��6   $	��Top=�6%   $"Выберите тип линии6R   �� 5)      -   5        $��LineTypesComboBox��5)   %   $��9��6   $ 0��6�   $	��Top=�6�   ��6v   �� *	LineTypes+6�   �� 5*      -   5        $��10��5*      $��0,95.8,-0.4,-95.8q�68   .��6T   Очистить6^   �� 5+      -   5        $��11��5+      .��6?   Удалить фигуру6O   5,      �� 5.   
   -   5        $��Canvas��5.       �� 
Background
 �����$��0.4,0,-7.6,-0.2q�6>   $��1��6W   5/      50      