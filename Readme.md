### Willkommen 👋
Herzlich Willkommen beim Projekt: Comparify.
Wir sind ein dreiköpfiges Team der technischen Hochschule Georg-Simon-Ohm und wollen Neueinsteigerinnen in der IT helfen, bzw. mit diesem Projekt einen Anreiz und Einblick in die Programmierung verschaffen.

### Installation
Benötigte Entwicklungsumgebung:
Visual Studio Community 2019: https://visualstudio.microsoft.com/de/vs/community/

###ListView.View

    <ListView.View>
      <GridView AllowsColumnReorder="False">
        <GridViewColumn x:Name="GridViewColumn_Webseite" Header="Plattform"  Width="150">
          <GridViewColumn.CellTemplate>
            <DataTemplate>
              <StackPanel Orientation="Horizontal">
                <Image x:Name="GridViewColumn_Plattform" Width="150" Height="100" Source="{Binding GridViewColumn_Plattform}" />
              </StackPanel>
            </DataTemplate>
          </GridViewColumn.CellTemplate>
        </GridViewColumn>
          <GridViewColumn x:Name="GridViewColumn_Produkt" Header="Produktbild"  Width="150">
              <GridViewColumn.CellTemplate>
                  <DataTemplate>
                      <StackPanel Orientation="Horizontal">
                          <Image x:Name="GridViewColumn_Bild" Width="150" Height="100" Source="{Binding GridViewColumn_Bild}" />
                      </StackPanel>
                  </DataTemplate>
              </GridViewColumn.CellTemplate>
          </GridViewColumn>
          <GridViewColumn x:Name="GridViewColumn_Titel" Header="Titel" Width="400" DisplayMemberBinding="{Binding GridViewColumn_Titel}" />
          <GridViewColumn x:Name="GridViewColumn_Preis" Header="Preis" Width="200" DisplayMemberBinding="{Binding GridViewColumn_Preis}" />
      </GridView>
  </ListView.View>
