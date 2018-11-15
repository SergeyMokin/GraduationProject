import { Text, List, Button, ListItem, Segment, InputGroup, Icon, Input, Content, Spinner, Item, Picker } from 'native-base';
import React, { Component } from 'react';
import styles from '../styles/mainstyle.js';
import ApiRequests from '../api/index.js';
import CameraPage from './camera-page.js';

const buttons = {
  blank: "Blank",
  type: "Type"
};

export default class BlankCreatorPage extends Component {
  constructor(props) {
    super(props);

    this.state = {
      isLoading: false,
      enabledButton: buttons.blank,
      typeName: "",
      imageData: null,
      selectedType: "Graduation Blank", //default
      selectedTemplate: "MainBlank",
      inputStyle: {
        color: '#4a76a8'
      },
      messageStyle: {
        color: '#ff4d4d',
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

  async componentWillMount() {
    await this.getTypes();
    await this.getTemplates();
  }

  async getTemplates() {
    this.setState({ isLoading: true });
    this.props.footerDisableCallback(true);
    let error = (error) => {
      console.log(error);
      this.setState({ isLoading: false });
      this.props.footerDisableCallback(false);
    };

    let success = (data) => {
      this.templates = data;
      this.setState({ isLoading: false });
      this.props.footerDisableCallback(false);
    };

    await this.api.getTemplates()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  async getTypes() {
    this.setState({ isLoading: true });
    this.props.footerDisableCallback(true);
    let error = (error) => {
      console.log(error);
      this.setState({ isLoading: false });
      this.props.footerDisableCallback(false);
    };

    let success = (data) => {
      this.types = data;
      this.onValueChangeType(data[0].name);
      this.setState({ isLoading: false });
      this.props.footerDisableCallback(false);
    };

    await this.api.getBlankTypes()
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  async addType() {
    this.setState({ isLoading: true });
    this.props.footerDisableCallback(true);
    let error = (error) => {
      this.successMessage = "";
      this.errorMessage = error.message;
      this.setState({
        isLoading: false,
        imageData: null,
        inputStyle: {
          color: '#ff4d4d'
        },
        messageStyle: {
          color: '#ff4d4d',
          alignSelf: 'center',
          padding: 20
        }
      });
      this.props.footerDisableCallback(false);
    };

    let success = async (data) => {
      this.errorMessage = "";
      this.successMessage = "Success";

      this.setState({
        isLoading: false,
        typeName: "",
        imageData: null,
        inputStyle: {
          color: '#4a76a8'
        },
        messageStyle: {
          color: 'green',
          alignSelf: 'center',
          padding: 20
        }
      });
      this.props.footerDisableCallback(false);
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

  onValueChangeType(value) {
    this.setState({
      selectedType: value
    });
  }

  onValueChangeTemplate(value) {
    this.setState({
      selectedTemplate: value
    });
  }

  async generateExcel() {
    if (this.state.imageData === null || this.state.isLoading) return;
    this.setState({ isLoading: true });
    this.props.footerDisableCallback(true);
    let error = (error) => {
      this.successMessage = "";
      this.errorMessage = error.status + " : " + error.message;
      this.setState({
        isLoading: false,
        messageStyle: {
          color: '#ff4d4d',
          alignSelf: 'center',
          padding: 20
        }
      });
      this.props.footerDisableCallback(false);
    };

    let success = async (data) => {
      this.errorMessage = "";
      this.successMessage = "Success";

      this.setState({
        isLoading: false,
        typeName: "",
        imageData: null,
        messageStyle: {
          color: 'green',
          alignSelf: 'center',
          padding: 20
        }
      });
      this.props.footerDisableCallback(false);
    };

    let uriParts = this.state.imageData.uri.split('.');
    let fileType = uriParts[uriParts.length - 1];
    let fileName = uriParts[uriParts.length - 2].split('/')[uriParts[uriParts.length - 2].split('/').length - 1];
    let type = `image/${fileType}`;
    let name = `${fileName}.${fileType}`

    await this.api.generateExcel({ id: 0, name: name, data: this.state.imageData.base64, type: this.state.selectedType, fileType: type, fileTypeUsers: [] })
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  refresh() {
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
      selectedType: this.types.length > 0 ? this.types[0].name : '',
      inputStyle: {
        color: '#4a76a8'
      },
      messageStyle: {
        color: '#ff4d4d',
        alignSelf: 'center',
        padding: 20
      }
    });
    this.props.footerDisableCallback(false);
  }

  render() {
    const content = this.state.isLoading ?
      <Content contentContainerStyle={styles.body}>
        <Spinner color="#4a76a8" />
      </Content>

      : this.state.enabledButton === buttons.blank && !this.state.isCamera && !this.state.isCameraRoll ?
        <Content>
          <Segment style={{ backgroundColor: "#4a76a8" }}>
            <Button bordered active={this.state.enabledButton === buttons.blank ? true : false} onPress={() => { this.setState({ enabledButton: buttons.blank }); this.refresh(); }}>
              <Text style={{ color: this.state.enabledButton === buttons.blank ? '#4a76a8' : 'white' }}>{buttons.blank}</Text>
            </Button>
            <Button bordered active={this.state.enabledButton === buttons.type ? true : false} onPress={() => { this.setState({ enabledButton: buttons.type }); this.refresh(); }}>
              <Text style={{ color: this.state.enabledButton === buttons.type ? '#4a76a8' : 'white' }}>{buttons.type}</Text>
            </Button>
          </Segment>
          <Text style={{ color: 'gray', padding: 5, alignSelf: 'center', marginTop: 10 }}>Select blank type</Text>
          <Item picker style={{ marginTop: 10 }}>
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
          <CameraPage imageCallback={(imageData) => { this.setState({ imageData: imageData }) }} />
          <Button style={styles.primaryButton} onPress={this.generateExcel.bind(this)}><Text>Generate excel</Text></Button>
          <Text style={this.state.messageStyle}>{this.errorMessage !== "" ? this.errorMessage : this.successMessage !== "" ? this.successMessage : ""}</Text>
        </Content>

        :
        <Content>
          <Segment style={{ backgroundColor: "#4a76a8" }}>
            <Button bordered active={this.state.enabledButton === buttons.blank ? true : false} onPress={() => { this.setState({ enabledButton: buttons.blank }); this.refresh(); }}>
              <Text style={{ color: this.state.enabledButton === buttons.blank ? '#4a76a8' : 'white' }}>{buttons.blank}</Text>
            </Button>
            <Button bordered active={this.state.enabledButton === buttons.type ? true : false} onPress={() => { this.setState({ enabledButton: buttons.type }); this.refresh(); }}>
              <Text style={{ color: this.state.enabledButton === buttons.type ? '#4a76a8' : 'white' }}>{buttons.type}</Text>
            </Button>
          </Segment>
          <List>
            <Text style={{ color: 'gray', padding: 5, alignSelf: 'center', marginTop: 10 }}>Select blank template</Text>
            <Item picker style={{ marginTop: 10 }}>
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
            <ListItem style={{ marginTop: 10 }}>
              <InputGroup>
                <Icon name="ios-document" style={this.state.inputStyle} />
                <Input
                  onChangeText={(text) => this.setState({ typeName: text })}
                  value={this.state.typeName}
                  placeholder={"Name of blank"} />
              </InputGroup>
            </ListItem>
            <CameraPage imageCallback={(imageData) => { this.setState({ imageData: imageData }) }} />
            <Button style={styles.primaryButton} onPress={this.addType.bind(this)}>
              <Text>Add blank type</Text>
            </Button>
            <Text style={this.state.messageStyle}>{this.errorMessage !== "" ? this.errorMessage : this.successMessage !== "" ? this.successMessage : ""}</Text>
          </List>
        </Content>
      ;

    return (content);
  }
}