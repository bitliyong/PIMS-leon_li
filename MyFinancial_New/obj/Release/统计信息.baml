   M S B A M L   `   `   `  ����   MyFinancial_New    MyFinancial.统计信息   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  统计信息��$	��500��$	��600�� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ SingleBorderWindow=� � 
ResizeMode$ NoResize=�.���� -   $��10,10,10,10q�.@��� -   $��statDetails��$��统计信息详情��.��� .+��� -   $��richTextBox1��$	��400��$
��Left=�$	��Top=�$	��578��$��0,40,0,0q� �� 
IsReadOnly . " �� VerticalScrollBarVisibility$
 Auto=�$��楷体)�$��16(�$
��Bold"����	#FFD72929���	#FFE6FFBE � DataContext �� �� -   $��label1��$!��我的financial统计信息��$��28��$
��Left=�$��
143,10,0,0q�$	��Top=�$	��204��$��楷体)�$
��Bold"�$��18(����	#FF00E7FE�� -   $��inOutDetails��$��收支明细��.���  � ColumnDefinitions �� $��* ��� $��2* ��� $��* ��� $��2* ��� $��* ��� $��2* � � RowDefinitions �� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��* ��� $��0.75* ��� $��0.75* ��� $��0.75* ��� $��0.75* �.+�&   System.Windows.Controls.DataGrid  -   $��	inOutGrid��$��0��$��8��$��0��$��6��$
��Left=�$��10,10,10,10q�$	��Top=�
������ 	   GridLinesVisibility$
	 None=�
   AutoGenerateColumns
 .     CanUserAddRows .     HeadersVisibility$ Column=�$��Center=�   
IsReadOnly .    CanUserSortColumns .    ColumnHeaderStyle ��  �� 
TargetType=  7System.Windows.Controls.Primitives.DataGridColumnHeader"  .���  �� Property � �� �� Value$ 30��   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 日期��   Width5  /System.Windows.Controls.DataGridLengthConverter$ 85 1  +System.Windows.Controls.DataGridBoundColumn   Binding ��  �� Path$
 Date�  $ 账户��$ *  �� $ Account�  $ 收入/支出��$ 70  �� $ InOut�  $ 金额（元）��$	 100  �� $ Count�  $ 备注��$ 2*  �� $ Comment��� $��8��$��0��$��	总收入���� -   $��	totalInLb��$��8��$��1��$
��0.00���� $��9��$��0��$��工资收入���� -	   $��
salaryInLb��$��9��$��1��$
��0.00���� $��9��$��2��$��收益收入���� -
   $��
profitInLb��$��9��$��3��$
��0.00���� $��9��$��4��$��其他收入���� -   $��	otherInLb��$��9��$��5��$
��0.00���� $��10��$��0��$��	总支出���� -   $��
totalOutLb��$��10��$��1��$
��0.00���� -   $��documentView��$��文本视图��.���� -   $��annual��$	��430��$
��Left=�$	��Top=�$	��578��$��0,0,0,0q� . $
 Auto=�$��楷体)�$��16(�$
��Bold"����	#FFD72929���	#FFE6FFBE �� �� -   $��listView��$��列表视图��.��  -   $��
annualGrid��$	��563��$	��425��$
��Left=�$��10,10,10,10q�$	��Top=�
������ $
	 None=�
 .   .  $ Column=�$��Center=� .  .  �� "  .���  � ��$ 30��   $ 年度��$ 50  �� $
 Name�  $ 起止时间��$ *  �� $ TIMESpan�  $ 目标��$	 145  �� $ Target�  $ 实现��$	 145  �� $ Current�  $ 	完成度��$ 60  �� $ Degree�