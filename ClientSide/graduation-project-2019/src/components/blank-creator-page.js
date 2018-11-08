import { Container, Text, List, Button, ListItem, Segment, InputGroup, Icon, Input, Content, Spinner, Item, Picker } from 'native-base';
import React, {Component} from 'react';
import styles from '../styles/mainstyle.js';
import ApiRequests from '../api/index.js';
import CameraPage from './camera-page.js';

const buttons = {
  blank: "Blank",
  type: "Type"
};

export default class BlankCreatorPage extends Component {
  constructor(props){
      super(props);

      this.state = {
        isLoading: false,
        enabledButton: buttons.blank,
        typeName: "",
        imageData: null,
        selectedType: "Graduation Blank", //default
        selectedTemplate: "MainBlank",
        inputStyle: {
          color: 'blue'
        },
        messageStyle:{
          color: 'red',
          alignSelf: 'center',
          padding: 20
        },
        linkDownloadedFile: null
      }

      this.api = new ApiRequests();
      this.api.setAuthorizationHeader(this.props.userInfo.bearerToken);
      
      this.types = [];
      this.templates = [];
      this.errorMessage = "";
      this.successMessage = "";
      this.typeToAdd = {
        typeName: "",
        questions: []
      }
  }

  async componentWillMount()
  {
    await this.getTypes();
    await this.getTemplates();
  }

  async getTemplates()
  {
    this.setState({isLoading: true});
    let error = (error) => {
      console.log(error);
      this.setState({isLoading: false});
    };

    let success = (data) => {
        this.templates = data;
        this.setState({isLoading: false});
    };

    await this.api.getTemplates()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  async getTypes()
  {
    this.setState({isLoading: true});
    let error = (error) => {
      console.log(error);
      this.setState({isLoading: false});
    };

    let success = (data) => {
        this.types = data;
        this.onValueChange2(data[0].name);
        this.setState({isLoading: false});
    };

    await this.api.getBlankTypes()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  async addType()
  {
    this.setState({isLoading: true});
    let error = (error) => {
        this.successMessage = "";
        this.errorMessage = error.message;
        this.setState({
            isLoading: false,
            imageData: null,
            inputStyle: {
                color: 'red'
            },
            messageStyle:{
              color: 'red',
              alignSelf: 'center',
              padding: 20
            }
        });
    };

    let success = async (data) => {
        this.errorMessage = "";
        this.successMessage = "Success";
        
        this.setState({
            isLoading: false,
            typeName: "",
            imageData: null,
            inputStyle: {
                color: 'blue'
            },
            messageStyle:{
              color: 'green',
              alignSelf: 'center',
              padding: 20
            }
          });
        await this.getTypes.call(this);
    };

    this.typeToAdd = {
        id: 0,
        type: this.state.selectedTemplate,
        blankTypeName: this.state.typeName,
        data: this.state.imageData.base64
    }
    await this.api.addBlankType(this.typeToAdd)
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  onValueChangeType(value)
  {
    this.setState({
        selectedType: value
      });
  }

  onValueChangeTemplate(value)
  {
    this.setState({
        selectedTemplate: value
      });
  }

  async generateExcel()
  {
    if(this.state.imageData === null || this.state.isLoading) return;
    this.setState({isLoading: true});
    let error = (error) => {
      this.successMessage = "";
        this.errorMessage = error.status + " : " + error.message;
        this.setState({
            isLoading: false,
            messageStyle:{
              color: 'red',
              alignSelf: 'center',
              padding: 20
            }
        });
    };

    let success = async (data) => {
        this.errorMessage = "";
        this.successMessage = "Success";
        
        this.setState({
            isLoading: false,
            typeName: "",
            imageData: null,
            messageStyle:{
              color: 'green',
              alignSelf: 'center',
              padding: 20
            }
          });
    };

    let uriParts = this.state.imageData.uri.split('.');
    let fileType = uriParts[uriParts.length - 1];
    let fileName = uriParts[uriParts.length - 2].split('/')[uriParts[uriParts.length - 2].split('/').length - 1];
    let type = `image/${fileType}`;
    let name = `${fileName}.${fileType}`

    await this.api.generateExcel({id: 0, name: name, data: this.state.imageData.base64, type: this.state.selectedType, fileType: type, fileTypeUsers: []})
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  refresh(){
    this.errorMessage = "";
    this.successMessage = "";
    this.typeToAdd = {
      typeName: "",
      questions: []
    }

    this.setState({
      isLoading: false,
      typeName: "",
      imageData: null,
      selectedType: this.types[0].name,
      inputStyle: {
        color: 'blue'
      },
      messageStyle:{
        color: 'red',
        alignSelf: 'center',
        padding: 20
      }
    });
  }

  render() {
    const content = this.state.isLoading ?
    <Content contentContainerStyle={styles.body}>
        <Spinner color="blue" />
    </Content>

    : this.state.enabledButton === buttons.blank && !this.state.isCamera && !this.state.isCameraRoll ?
    <Content>
        <Text style={{color:'gray', padding: 5, alignSelf: 'center', marginTop: 10}}>Select blank type</Text>
        <Item picker style={{marginTop: 10}}>
              <Picker
                mode="dropdown"
                iosIcon={<Icon name="ios-arrow-down-outline" />}
                style={{ width: undefined }}
                placeholder="Select blank type"
                placeholderStyle={{ color: "#bfc6ea" }}
                placeholderIconColor="#007aff"
                selectedValue={this.state.selectedType}
                onValueChange={this.onValueChangeType.bind(this)}
              >
              {this.types.map(item => <Picker.Item label={item.name} value={item.name} key={item.id} />)}
              </Picker>
        </Item>
        <CameraPage imageCallback = {(imageData) => {this.setState({imageData: imageData})}}/>
        <Button style={styles.primaryButton} onPress={this.generateExcel.bind(this)}><Text>Generate excel</Text></Button>
        <Text style={this.state.messageStyle}>{this.errorMessage !== "" ? this.errorMessage : this.successMessage !== "" ? this.successMessage : ""}</Text>
    </Content>

    :
          <List>
            <Text style={{color:'gray', padding: 5, alignSelf: 'center', marginTop: 10}}>Select blank template</Text>
            <Item picker style={{marginTop: 10}}>
                    <Picker
                    mode="dropdown"
                    iosIcon={<Icon name="ios-arrow-down-outline" />}
                    style={{ width: undefined }}
                    placeholder="Select blank template"
                    placeholderStyle={{ color: "#bfc6ea" }}
                    placeholderIconColor="#007aff"
                    selectedValue={this.state.selectedTemplate}
                    onValueChange={this.onValueChangeTemplate.bind(this)}
                    >
                    {this.templates.map(item => <Picker.Item label={item} value={item} key={item} />)}
                    </Picker>
            </Item>
            <ListItem style={{marginTop: 10}}>
                <InputGroup>
                    <Icon name="ios-document" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({typeName: text})}
                        value={this.state.typeName}
                        placeholder={"Name of blank"} />
                </InputGroup>
            </ListItem> 
            <CameraPage imageCallback = {(imageData) => {this.setState({imageData: imageData})}}/>
            <Button style={styles.primaryButton} onPress={this.addType.bind(this)}>
                <Text>Add blank type</Text>
            </Button>
            <Text style={this.state.messageStyle}>{this.errorMessage !== "" ? this.errorMessage : this.successMessage !== "" ? this.successMessage : ""}</Text>
          </List>                
    ;

    return (
            <Container>
              <Segment style={{backgroundColor:"blue"}}>
                  <Button bordered active={this.state.enabledButton === buttons.blank ? true : false} onPress={() => {this.setState({enabledButton: buttons.blank}); this.refresh();}}>
                        <Text>{buttons.blank}</Text>
                  </Button>              
                  <Button bordered active={this.state.enabledButton === buttons.type ? true : false} onPress={() => {this.setState({enabledButton: buttons.type}); this.refresh();}}>
                        <Text>{buttons.type}</Text>
                  </Button>   
              </Segment>
              <Content>
                {content}
              </Content>
            </Container>
    );
  }
}