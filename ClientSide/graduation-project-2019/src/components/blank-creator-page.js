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
        q1: "",
        q2: "",
        q3: "",
        q4: "",
        q5: "",
        q6: "",
        q7: "",
        q8: "",
        q9: "",
        q10: "",
        q11: "",
        q12: "",
        q13: "",
        q14: "",
        q15: "",
        q16: "",
        q17: "",
        imageData: null,
        selected2: "Graduation Project",
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
        this.onValueChange2(data[0].type);
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
            q1: "",
            q2: "",
            q3: "",
            q4: "",
            q5: "",
            q6: "",
            q7: "",
            q8: "",
            q9: "",
            q10: "",
            q11: "",
            q12: "",
            q13: "",
            q14: "",
            q15: "",
            q16: "",
            q17: "",
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
      typeName: this.state.typeName,
      questions: [this.state.q1, this.state.q2, this.state.q3, this.state.q4, this.state.q5, this.state.q6, this.state.q7,
         this.state.q8, this.state.q9, this.state.q10, this.state.q11, this.state.q12, this.state.q13, this.state.q14, this.state.q15, this.state.q16, this.state.q17]
    }
    await this.api.addBlankType(this.typeToAdd)
      .then(success.bind(this))
      .catch(error.bind(this));
  }

  onValueChange2(value)
  {
    this.setState({
        selected2: value
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

    await this.api.generateExcel({id: 0, name: name, data: this.state.imageData.base64, type: this.state.selected2, fileType: type, fileTypeUsers: []})
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
      q1: "",
      q2: "",
      q3: "",
      q4: "",
      q5: "",
      q6: "",
      q7: "",
      q8: "",
      q9: "",
      q10: "",
      q11: "",
      q12: "",
      q13: "",
      q14: "",
      q15: "",
      q16: "",
      q17: "",
      selected2: this.types[0].type,
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
        <Item picker>
              <Picker
                mode="dropdown"
                iosIcon={<Icon name="ios-arrow-down-outline" />}
                style={{ width: undefined }}
                placeholder="Select blank type"
                placeholderStyle={{ color: "#bfc6ea" }}
                placeholderIconColor="#007aff"
                selectedValue={this.state.selected2}
                onValueChange={this.onValueChange2.bind(this)}
              >
              {this.types.map(item => <Picker.Item label={item.type} value={item.type} key={item.id} />)}
              </Picker>
        </Item>
        <CameraPage imageCallback = {(imageData) => {this.setState({imageData: imageData})}}/>
        <Button style={styles.primaryButton} onPress={this.generateExcel.bind(this)}><Text>Generate excel</Text></Button>
        <Text style={this.state.messageStyle}>{this.errorMessage !== "" ? this.errorMessage : this.successMessage !== "" ? this.successMessage : ""}</Text>
    </Content>

    :
          <List>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-document" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({typeName: text})}
                        value={this.state.typeName}
                        placeholder={"Name of type"} />
                </InputGroup>
            </ListItem> 
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q1: text})}
                        value={this.state.q1}
                        placeholder={"Q1"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q2: text})}
                        value={this.state.q2}
                        placeholder={"Q2"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q3: text})}
                        value={this.state.q3}
                        placeholder={"Q3"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q4: text})}
                        value={this.state.q4}
                        placeholder={"Q4"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q5: text})}
                        value={this.state.q5}
                        placeholder={"Q5"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q6: text})}
                        value={this.state.q6}
                        placeholder={"Q6"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q7: text})}
                        value={this.state.q7}
                        placeholder={"Q7"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q8: text})}
                        value={this.state.q8}
                        placeholder={"Q8"} />
                </InputGroup>
            </ListItem>      
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q9: text})}
                        value={this.state.q9}
                        placeholder={"Q9"} />
                </InputGroup>
            </ListItem>   
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q10: text})}
                        value={this.state.q10}
                        placeholder={"Q10"} />
                </InputGroup>
            </ListItem>      
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q11: text})}
                        value={this.state.q11}
                        placeholder={"Q11"} />
                </InputGroup>
            </ListItem>   
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q12: text})}
                        value={this.state.q12}
                        placeholder={"Q12"} />
                </InputGroup>
            </ListItem>   
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q13: text})}
                        value={this.state.q13}
                        placeholder={"Q13"} />
                </InputGroup>
            </ListItem>  
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q14: text})}
                        value={this.state.q14}
                        placeholder={"Q14"} />
                </InputGroup>
            </ListItem>  
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q15: text})}
                        value={this.state.q15}
                        placeholder={"Q15"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q16: text})}
                        value={this.state.q16}
                        placeholder={"Q16"} />
                </InputGroup>
            </ListItem>
            <ListItem style={{height: 20}}>
                <InputGroup>
                    <Icon name="ios-help" style={this.state.inputStyle} />
                    <Input
                        onChangeText={(text) => this.setState({q17: text})}
                        value={this.state.q17}
                        placeholder={"Q17"} />
                </InputGroup>
            </ListItem>
              <Button style={styles.primaryButton} onPress={this.addType.bind(this)}>
                  <Text>Add type</Text>
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