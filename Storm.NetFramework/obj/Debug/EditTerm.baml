   M S B A M L   `   `   `  ����M  IStorm.NetFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:Storm.NetFrameworkStorm.NetFramework  5   	   !    Storm.NetFramework.EditTerm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:Storm.NetFramework     5   	     � Title$  EditTerm��5   	   $	��240��6   $	��240��6'   $	��430��67   $	��430��6C    � WindowStartupLocation$ CenterScreen=�6R    � 
ResizeMode$ CanResizeWithGrip=�6w   .��5	      � -   5        $��gridMain��5	       � ColumnDefinitions 5
   
   �� 5      $
��110* �6   $	��140��6,   �� 5      $
��301* �6    � RowDefinitions 5   
   �� 5      $	��25* �6   �� 5      $	��25* �6   �� 5      $	��25* �6   �� 5      $	��25* �6   �� 5      $	��25* �6   �� 5      $	��25* �6   �� 5      $	��35* �6   .+�5   
   �� -   5        $��LabelTextBox1��5      $��10,0,0,0q�6%   $��Center=�67   $��28��6R   $
��Bold"�6^   �� 5   
   -   5        $��LabelTextBox2��5      $��10,0,0,0q�6%   $��Center=�67   $��28��6R   $
��Bold"�6^   $��1��6p   �� 5   
   -   5        $��LabelComboBox1��5      $��10,0,0,0q�6&   $��2��68   $��Center=�6E   $��28��6`   $
��Bold"�6l   �� 5   
   -   5        $��LabelComboBox2��5      $��10,0,0,0q�6&   $��3��68   $��Center=�6E   $��28��6`   $
��Bold"�6l   �� 5   
   -   5        $��LabelComboBox3��5      $��10,0,0,0q�6&   $��4��68   $��Center=�6E   $��28��6`   $
��Bold"�6l   �� 5   
   -   5        $��LabelDateTimePicker1��5      $��10,0,0,0q�6,   $��5��6>   $��Center=�6K   $��29��6f   $
��Bold"�6r   �� 5   
   -   5        $��TextBox1��5      $��1��6"   $��24��62   $��	10,1,10,3q�6>   $��Center=�6Q    �� TextWrapping$ NoWrap=�6l    �� 	MaxLength$	 255��6�   �� 5   
   -	   5        $��TextBox2��5      $��1��6"   $��24��62   $
��10,2q�6>   $��1��6L   $ NoWrap=�6Y   $��Center=�6o   $	 255��6�   �� 5   
   -
   5        $��	ComboBox1��5      $��1��6$   $��	10,4,10,3q�64   $��2��6G   $��Center=�6T   $��22��6o   ��6{   �� �� 5    
   -   5        $��	ComboBox2��5       $��1��6$   $��	10,3,10,3q�64   $��3��6G   $��Center=�6T   $��22��6o   ��6{   �� �� 5!   
   -   5        $��	ComboBox3��5!      $��1��6$   $��	10,3,10,4q�64   $��4��6G   $��Center=�6T   $��22��6o   ��6{   �� �� 5"   
   -   5        $��Сохранить��5"   (   $��6��6<   $��Right=�6I   $	��120��6e   $��23��6q   $��Bottom=�6}   $��	0,0,10,10q�6�   $��1��6�    �� 	IsDefault . 6�   (  "System.Windows.Controls.DatePicker  5#   
   -   5        $��DatePicker1��5#      $��1��6(   $��	10,3,10,4q�68   $��5��6K   $��Center=�6X   5$      5%      